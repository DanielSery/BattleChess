using BattleChess3.Game;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Helpers;
using BattleChess3.Game.Players;
using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

internal sealed class MultiplayerGameService : IMultiplayerGameService
{
    private readonly IMultiplayerScheduler _scheduler;
    
    private readonly IMongoCollection<GameTurn> _gameTurnsCollection;
    private readonly IMongoCollection<RegisteredPlayer> _playersCollection;
    private readonly IMultiplayerPlayerService _playerService;
    private readonly MongoClient _client;

    public MultiplayerGameService(
        IMultiplayerScheduler scheduler,
        IMultiplayerPlayerService playerService)
    {
        _scheduler = scheduler;
        _playerService = playerService;
        
        _client = new MongoClient(Secrets.ConnectionString);
        var database = _client.GetDatabase("BattleChess");
        _gameTurnsCollection = database.GetCollection<GameTurn>("GameTurns");
        _playersCollection = database.GetCollection<RegisteredPlayer>("Players");
    }

    public event EventHandler<(Position, Position, TimeSpan)>? RequestPlayMove;

    private MultiplayerGameType GameType { get; set; }
    private string? GameId { get; set; }
    private string? TurnId { get; set; }

    public void StartGame(MultiplayerGameType gameType, string? rankedGameId)
    {
        GameType = gameType;
        GameId = rankedGameId;
        TurnId = null;
    }

    public Task<Result<string?>> HandleWinAsync(bool nofityOther, WinType winType, IOnlinePlayerInfo won, IOnlinePlayerInfo lost)
    {
        lock (_scheduler.SyncLock)
        {
            if (GameId is null)
                return Task.FromResult(Result.Ok<string?>(null));
        
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    if (GameId is null)
                    {
                        return Result.Ok<string?>(null);
                    }
                    
                    await DeleteGameTurnsAsync();
                    if (nofityOther)
                    {
                        await SendGameResultToOpponent(winType);
                    }

                    if (!GameType.HasFlag(MultiplayerGameType.Ranked))
                    {
                        return Result.Ok<string?>(null);
                    }

                    if (nofityOther)
                    {
                        return await UpdatePlayersElo(won, lost);
                    }
                    else
                    {
                        var updated = _playerService.LoggedInPlayer!.Id == won.PlayerId ? won : lost; 
                        return await GetUpdatedElo(updated);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail(ex.Message);
                } 
            });
        }
    }

    private async Task SendGameResultToOpponent(WinType winType)
    {
        if (GameId == null)
            return;
        
        if (winType == WinType.CapturedKing)
            return;
        
        Console.WriteLine("Sending game result to the other player");
        int messageIndex = winType switch
        {
            WinType.Surrender => IMultiplayerGameService.SurrenderMessage,
            WinType.NotResponding => IMultiplayerGameService.NotRespondingLostMessage,
            WinType.OutOfTime => IMultiplayerGameService.OutOfTimeMessage,
            _ => throw new ArgumentOutOfRangeException(nameof(winType), winType, null)
        };

        try
        {
            Console.WriteLine("Creating game result");
            var gameTurn = new GameTurn()
            {
                GameId = GameId,
                FromIndex = (byte)messageIndex,
                ToIndex = 0,
                CreatedAt = DateTime.UtcNow,
                TimeSpentInSeconds = 0
            };
            await _gameTurnsCollection.InsertOneAsync(gameTurn);
            Console.WriteLine("Created game result");        
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task<Result<string?>> GetUpdatedElo(IOnlinePlayerInfo lost)
    {
        Console.WriteLine($"Searching for losing player with id: {lost.PlayerId}");
        var updatedPlayerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, lost.PlayerId);
        var updatedPlayers = await _playersCollection.FindAsync(updatedPlayerFilter);
        var updatedPlayer = await updatedPlayers.FirstOrDefaultAsync();
        if (updatedPlayer is null)
        {
            return Result.Fail("Could not find losing player");
        }
        Console.WriteLine($"Found guest player with id: {updatedPlayer.Id}");

        if (updatedPlayer.Elo != lost.Elo)
        {
            _playerService.LoggedInPlayer!.Elo = updatedPlayer.Elo;
            return Result.Ok<string?>($"Elo {updatedPlayer.Elo - lost.Elo} → {updatedPlayer.Elo}");
        }

        updatedPlayer = await WaitForEloUpdate(lost);
        if (updatedPlayer is null)
        {
            return Result.Fail("Could not find losing player");
        }

        _playerService.LoggedInPlayer!.Elo = updatedPlayer.Elo;
        return Result.Ok<string?>($"Elo {updatedPlayer.Elo - lost.Elo} → {updatedPlayer.Elo}");
    }

    private async Task<RegisteredPlayer?> WaitForEloUpdate(IOnlinePlayerInfo lost)
    {
        try
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RegisteredPlayer>>()
                .Match(change =>
                    (change.OperationType == ChangeStreamOperationType.Update &&
                     change.DocumentKey["_id"] == ObjectId.Parse(lost.PlayerId)));
        
            using var cursor = await _playersCollection.WatchAsync(
                pipeline,
                new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
                cancellationTokenSource.Token
            );

            while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
            {
                foreach (var change in cursor.Current)
                {
                    if (change.FullDocument.Id == lost.PlayerId)
                    {
                        return change.FullDocument;
                    }
                }
            }

            return null;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    private async Task<Result<string?>> UpdatePlayersElo(IOnlinePlayerInfo won, IOnlinePlayerInfo lost)
    {
        Console.WriteLine($"Searching for winning player with id: {won.PlayerId}");
        var winningPlayerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, won.PlayerId);
        var winningPlayers = await _playersCollection.FindAsync(winningPlayerFilter);
        var winningPlayer = await winningPlayers.FirstOrDefaultAsync();
        if (winningPlayer is null)
        {
            return Result.Fail("Could not find winning player");
        }
        winningPlayer.Elo = (short)won.Elo!;
        Console.WriteLine($"Found winning player with id: {winningPlayer.Id}");
                    
        Console.WriteLine($"Searching for losing player with id: {lost.PlayerId}");
        var losingPlayerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, lost.PlayerId);
        var losingPlayers = await _playersCollection.FindAsync(losingPlayerFilter);
        var losingPlayer = await losingPlayers.FirstOrDefaultAsync();
        if (losingPlayer is null)
        {
            return Result.Fail("Could not find losing player");
        }
        losingPlayer.Elo = (short)lost.Elo!;
        Console.WriteLine($"Found guest player with id: {losingPlayer.Id}");

        UpdateElo(winningPlayer, losingPlayer, 1d);
                        
        var updateWinningPlayer = Builders<RegisteredPlayer>.Update.Set(x => x.Elo, winningPlayer.Elo);
        var updateWinningPlayerResult = await _playersCollection.UpdateOneAsync(winningPlayerFilter, updateWinningPlayer);
                    
        var updateLosingPlayer = Builders<RegisteredPlayer>.Update.Set(x => x.Elo, losingPlayer.Elo);
        var updateLosingPlayerResult = await _playersCollection.UpdateOneAsync(losingPlayerFilter, updateLosingPlayer);

        if (!updateLosingPlayerResult.IsAcknowledged)
        {
            return Result.Fail("Could not update guest player");
        }
                    
        if (!updateWinningPlayerResult.IsAcknowledged)
        {
            return Result.Fail("Failed to update game confirmation");
        }

        var currentPlayer = _playerService.LoggedInPlayer!.Id == won.PlayerId ? won : lost;
        var updatedPlayer = _playerService.LoggedInPlayer!.Id == winningPlayer.Id ? winningPlayer : losingPlayer;
        _playerService.LoggedInPlayer!.Elo = updatedPlayer.Elo;
        return Result.Ok<string?>($"Elo {updatedPlayer.Elo - currentPlayer.Elo} → {updatedPlayer.Elo}");
    }

    private static void UpdateElo(RegisteredPlayer playerA, RegisteredPlayer playerB, double resultA, int k = 32)
    {
        var expectedA = 1.0 / (1.0 + Math.Pow(10, (playerB.Elo - playerA.Elo) / 400.0));
        var expectedB = 1.0 / (1.0 + Math.Pow(10, (playerA.Elo - playerB.Elo) / 400.0));

        playerA.Elo += (short)(k * (resultA - expectedA));
        playerB.Elo += (short)(k * ((1 - resultA) - expectedB));
    }

    public Task<Result> PlayedMoveAsync(Position from, Position to, TimeSpan timeSpent)
    {
        lock (_scheduler.SyncLock)
        {
            if (GameId is null)
                return Task.FromResult(Result.Fail("Not in game"));
        
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Creating game turn: {from} to {to}");
                    var gameTurn = new GameTurn()
                    {
                        GameId = GameId,
                        FromIndex = (byte)from.GetIndex(),
                        ToIndex = (byte)to.GetIndex(),
                        CreatedAt = DateTime.UtcNow,
                        TimeSpentInSeconds = timeSpent.TotalSeconds
                    };
                    await _gameTurnsCollection.InsertOneAsync(gameTurn);
                    Console.WriteLine($"Created game turn: {from} to {to}");

                    TurnId = gameTurn.Id;
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail(ex.Message);
                } 
            });
        }
    }

    public Task<Result> HandleHisTurnAsync()
    {
        lock (_scheduler.SyncLock)
        {
            if (GameId is null)
                return Task.FromResult(Result.Fail("Not in game"));
        
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Waiting for his turn with id greater than: {TurnId}");
                    try
                    {
                        var hisTurn = TurnId is null
                            ? await WaitForNextTurnAsync(GameId, IMultiplayerGameService.TurnTimeout)
                            : await WaitForNextTurnAsync(TurnId, GameId, IMultiplayerGameService.TurnTimeout);       
                        
                        if (hisTurn is null)
                        {
                            RequestPlayMove?.Invoke(this, 
                                (Position.FromIndex(IMultiplayerGameService.NotRespondingMessage), 
                                    Position.None, 
                                    TimeSpan.FromMinutes(2)));
                            return Result.Ok();
                        }
                        
                        Console.WriteLine($"Found his turn with id: {hisTurn.Id}");

                        if (hisTurn.FromIndex >= 64)
                        {
                            RequestPlayMove?.Invoke(this, new ValueTuple<Position, Position, TimeSpan>( 
                                Position.FromIndex(hisTurn.FromIndex), 
                                Position.FromIndex(hisTurn.ToIndex), 
                                TimeSpan.FromSeconds(hisTurn.TimeSpentInSeconds)));
                            return Result.Ok();
                        }

                        RequestPlayMove?.Invoke(this, new ValueTuple<Position, Position, TimeSpan>(
                            GetPositionOfOppositePlayer(hisTurn.FromIndex),
                            GetPositionOfOppositePlayer(hisTurn.ToIndex),
                            TimeSpan.FromSeconds(hisTurn.TimeSpentInSeconds)));          
                        return Result.Ok();   
                    }
                    catch (OperationCanceledException)
                    {
                        RequestPlayMove?.Invoke(this, 
                            (Position.FromIndex(IMultiplayerGameService.NotRespondingMessage), 
                                Position.None, 
                                TimeSpan.FromMinutes(2)));
                        return Result.Ok();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail(ex.Message);
                } 
            });
        }
    }

    private async Task<GameTurn?> WaitForNextTurnAsync(string gameId, TimeSpan timeout)
    {
        try
        {
            var cancellationTokenSource = new CancellationTokenSource(timeout);
            var filter = Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId);

            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameTurn>>()
                .Match(filter);

            using var cursor = await _gameTurnsCollection.WatchAsync(
                pipeline,
                new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
                cancellationTokenSource.Token
            );

            while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
            {
                foreach (var change in cursor.Current)
                {
                    var turn = change.FullDocument;
                    if (turn.GameId == gameId)
                    {
                        return turn;
                    }
                }
            }

            return null;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    private async Task<GameTurn?> WaitForNextTurnAsync(string turnId, string gameId, TimeSpan timeout) 
    {
        try
        {
            var afterObjectId = ObjectId.Parse(turnId);
            var cancellationTokenSource = new CancellationTokenSource(timeout);

            var filter = Builders<ChangeStreamDocument<GameTurn>>.Filter.And(
                Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId),
                Builders<ChangeStreamDocument<GameTurn>>.Filter.Gt(cs => cs.FullDocument.Id, turnId)
            );

            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameTurn>>()
                .Match(filter);

            using var cursor = await _gameTurnsCollection.WatchAsync(
                pipeline,
                new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
                cancellationTokenSource.Token
            );

            while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
            {
                foreach (var change in cursor.Current)
                {
                    var turn = change.FullDocument;
                    if (turn.GameId == gameId && ObjectId.Parse(turn.Id) > afterObjectId)
                    {
                        return turn;
                    }
                }
            }

            return null;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    private static Position GetPositionOfOppositePlayer(int index)
    {
        return RelativePositionHelper.GetRelative(Player.White, Position.FromIndex(index));
    }

    private async Task<Result> DeleteGameTurnsAsync()
    {
        try
        {
            Console.WriteLine("Getting server time");
            var serverTime = await GetServerTimeAsync();
            Console.WriteLine($"Current server time: {serverTime}");

            var oldestKeepTime = serverTime - TimeSpan.FromMinutes(20);
            
            Console.WriteLine("Deleting GameTurns");
            var filter = Builders<GameTurn>.Filter.Lt(gj => gj.CreatedAt, oldestKeepTime);
            var result = await _gameTurnsCollection.DeleteManyAsync(filter);
            Console.WriteLine($"Deleted GameTurns: {result.DeletedCount}");
            return Result.Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return Result.Fail(ex.Message);
        }
    }

    private async Task<DateTime> GetServerTimeAsync()
    {
        var command = new BsonDocument("hello", 1); // "hello" is the modern replacement for "isMaster"
        var result = await _client.GetDatabase("admin").RunCommandAsync<BsonDocument>(command);
        return result["localTime"].ToUniversalTime();
    }
}