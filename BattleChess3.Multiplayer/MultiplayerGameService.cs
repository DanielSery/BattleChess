using BattleChess3.Game.Board;
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

    public MultiplayerGameService(
        IMultiplayerScheduler scheduler,
        IMultiplayerPlayerService playerService)
    {
        _scheduler = scheduler;
        _playerService = playerService;
        
        var client = new MongoClient(Secrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
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

    public Task<Result<string?>> HandleWinAsync(bool nofityOther, WinType winType, Player won, Player lost)
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

                    if (nofityOther)
                    {
                        Console.WriteLine("Sending game result to the other player");
                        var messageIndex = winType switch
                        {
                            WinType.CapturedKing => won.Index == 1 ? IMultiplayerGameService.WonMessage : IMultiplayerGameService.LostMessage,
                            WinType.Surrender => IMultiplayerGameService.SurrenderMessage,
                            WinType.NotResponding => IMultiplayerGameService.NotRespondingMessage,
                            WinType.OutOfTime => IMultiplayerGameService.OutOfTimeMessage,
                            _ => throw new ArgumentOutOfRangeException(nameof(winType), winType, null),
                        };
                        
                        var result = await PlayedMoveAsync(Position.FromIndex(messageIndex), Position.None, TimeSpan.Zero);
                        if (result.IsFailed)
                        {
                            Console.WriteLine("Could not send game result to the other player");
                        }
                        Console.WriteLine("Sent game result to the other player");
                    }

                    if (!GameType.HasFlag(MultiplayerGameType.Ranked))
                    {
                        return Result.Ok<string?>(null);
                    }
                    
                    Console.WriteLine($"Searching for winning player with id: {won.PlayerId}");
                    var winningPlayerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, won.PlayerId);
                    var winningPlayers = await _playersCollection.FindAsync(winningPlayerFilter);
                    var winningPlayer = await winningPlayers.FirstOrDefaultAsync();
                    if (winningPlayer is null)
                    {
                        return Result.Fail("Could not find winning player");
                    }
                    Console.WriteLine($"Found winning player with id: {winningPlayer.Id}");
                    
                    Console.WriteLine($"Searching for losing player with id: {lost.PlayerId}");
                    var losingPlayerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, lost.PlayerId);
                    var losingPlayers = await _playersCollection.FindAsync(losingPlayerFilter);
                    var losingPlayer = await losingPlayers.FirstOrDefaultAsync();
                    if (losingPlayer is null)
                    {
                        return Result.Fail("Could not find losing player");
                    }
                    Console.WriteLine($"Found guest player with id: {losingPlayer.Id}");

                    UpdateElo(winningPlayer, losingPlayer, GameType.HasFlag(MultiplayerGameType.Host) ? 1d : 0d);
                    if (won.Index == 1) // only update database if winning player
                    {
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
                        
                        _playerService.LoggedInPlayer!.Elo = winningPlayer.Elo;
                    }
                    else
                    {
                        _playerService.LoggedInPlayer!.Elo = losingPlayer.Elo;
                    }
                    
                    return Result.Ok<string?>($"{won.Name} gained {winningPlayer.Elo - won.Elo} → {winningPlayer.Elo}\n" +
                                              $"{lost.Name} lost {losingPlayer.Elo - lost.Elo} → {losingPlayer.Elo}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail(ex.Message);
                } 
            });
        }
    }
    public static void UpdateElo(RegisteredPlayer playerA, RegisteredPlayer playerB, double resultA, int k = 32)
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
                        FromIndex = (byte)from.Index,
                        ToIndex = (byte)to.Index,
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

    private async Task<GameTurn?> WaitForNextTurnAsync(string turnId, string gameId, TimeSpan timeout) 
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

    private static Position GetPositionOfOppositePlayer(int index)
    {
        return Position.FromIndex(index)
            .GetPlayerPOVPosition(1);
    }

    public Task<Result> DeleteGameAsync(string gameId)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine("Deleting GameTurns");
                    var filter = Builders<GameTurn>.Filter.Eq(gj => gj.GameId, gameId);
                    var result = await _gameTurnsCollection.DeleteManyAsync(filter);
                    Console.WriteLine($"Deleted GameTurns: {result.DeletedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                return Result.Ok();
            });
        }
    }
}