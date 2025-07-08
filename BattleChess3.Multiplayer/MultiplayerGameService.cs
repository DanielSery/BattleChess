using BattleChess3.Game.Board;
using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

internal sealed class MultiplayerGameService : IMultiplayerGameService
{
    private readonly IMultiplayerScheduler _scheduler;
    
    private readonly IMongoCollection<GameTurn> _gameTurnsCollection;
    private readonly IMongoCollection<RankedGame> _rankedGamesCollection;
    private readonly IMongoCollection<RankedGameJoins> _rankedGameJoinsCollection;
    private readonly IMongoCollection<Player> _playersCollection;

    public MultiplayerGameService(
        IMultiplayerScheduler scheduler)
    {
        _scheduler = scheduler;
        
        var client = new MongoClient(DbSecrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _gameTurnsCollection = database.GetCollection<GameTurn>("GameTurns");
        _rankedGamesCollection = database.GetCollection<RankedGame>("RankedGames");
        _rankedGameJoinsCollection = database.GetCollection<RankedGameJoins>("RankedGameJoins");
        _playersCollection = database.GetCollection<Player>("Players");
    }

    public event EventHandler<(Position, Position)>? RequestPlayMove;

    private MultiplayerGameType GameType { get; set; }
    private string? GameId { get; set; }
    private string? TurnId { get; set; }

    public void StartGame(MultiplayerGameType gameType, string? rankedGameId)
    {
        GameType = gameType;
        GameId = rankedGameId;
        TurnId = null;
    }

    public Task<Result> HandleWinAsync()
    {
        lock (_scheduler.SyncLock)
        {
            if (GameId is null)
                return Task.FromResult(Result.Fail("Not in game"));
        
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    if (GameId is null || !GameType.HasFlag(MultiplayerGameType.Ranked))
                    {
                        return Result.Ok();
                    }
                    
                    Console.WriteLine($"Searching for game with id: {GameId}");
                    var filter = Builders<RankedGame>.Filter.Eq(g => g.Id, GameId);
                    var foundGames = await _rankedGamesCollection.FindAsync(filter);
                    var foundGame = foundGames.FirstOrDefault();
                    if (foundGame is null)
                    {
                        return Result.Fail("Could not find game");
                    }
                    Console.WriteLine($"Found game with id: {foundGame.Id}");
                    
                    Console.WriteLine($"Searching for game join with id: {foundGame.JoinedId}");
                    var joinFilter = Builders<RankedGameJoins>.Filter.Eq(g => g.GameId, GameId);
                    var foundJoins = await _rankedGameJoinsCollection.FindAsync(joinFilter);
                    var foundJoin = foundJoins.FirstOrDefault();
                    if (foundJoin is null)
                    {
                        return Result.Fail("Could opponent");
                    }
                    Console.WriteLine($"Found game join with id: {foundJoin.Id}");
                    
                    Console.WriteLine($"Searching for host player with id: {foundGame.PlayerId}");
                    var hostPlayerFilter = Builders<Player>.Filter.Eq(g => g.Id, foundGame.PlayerId);
                    var hostPlayers = await _playersCollection.FindAsync(hostPlayerFilter);
                    var hostPlayer = hostPlayers.FirstOrDefault();
                    if (hostPlayer is null)
                    {
                        return Result.Fail("Could not find host player");
                    }
                    Console.WriteLine($"Found host player with id: {foundJoin.Id}");
                    
                    Console.WriteLine($"Searching for guest player with id: {foundJoin.PlayerId}");
                    var guestPlayerFilter = Builders<Player>.Filter.Eq(g => g.Id, foundJoin.PlayerId);
                    var guestPlayers = await _playersCollection.FindAsync(guestPlayerFilter);
                    var guestPlayer = guestPlayers.FirstOrDefault();
                    if (guestPlayer is null)
                    {
                        return Result.Fail("Could not find guest player");
                    }
                    Console.WriteLine($"Found guest player with id: {foundJoin.Id}");

                    UpdateElo(hostPlayer, guestPlayer, GameType.HasFlag(MultiplayerGameType.Host) ? 1d : 0d);
                    var hostPlayerUpdate = Builders<Player>.Update.Set(x => x.Elo, hostPlayer.Elo);
                    var hostPlayerResult = await _playersCollection.UpdateOneAsync(hostPlayerFilter, hostPlayerUpdate);
                    
                    var guestPlayerUpdate = Builders<Player>.Update.Set(x => x.Elo, guestPlayer.Elo);
                    var guestPlayerResult = await _playersCollection.UpdateOneAsync(guestPlayerFilter, guestPlayerUpdate);

                    if (!guestPlayerResult.IsAcknowledged)
                    {
                        return Result.Fail("Could not update guest player");
                    }
                    
                    if (!hostPlayerResult.IsAcknowledged)
                    {
                        return Result.Fail("Failed to update game confirmation");
                    }
                    
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
    public static void UpdateElo(Player playerA, Player playerB, double resultA, int k = 32)
    {
        var expectedA = 1.0 / (1.0 + Math.Pow(10, (playerB.Elo - playerA.Elo) / 400.0));
        var expectedB = 1.0 / (1.0 + Math.Pow(10, (playerA.Elo - playerB.Elo) / 400.0));

        playerA.Elo += (short)(k * (resultA - expectedA));
        playerB.Elo += (short)(k * ((1 - resultA) - expectedB));
    }

    public Task<Result> PlayedMoveAsync(Position from, Position to)
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
                        ToIndex = (byte)to.Index
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
                    var hisTurn = TurnId is null
                        ? await WaitForNextTurnAsync(GameId)
                        : await WaitForNextTurnAsync(TurnId, GameId);
                    if (hisTurn is null)
                    {
                        return  Result.Fail("Opponent did not play in time.");
                    }
                    Console.WriteLine($"Found his turn with id: {hisTurn.Id}");

                    RequestPlayMove?.Invoke(this, new ValueTuple<Position, Position>(
                        GetPositionOfOppositePlayer(hisTurn.FromIndex),
                        GetPositionOfOppositePlayer(hisTurn.ToIndex)));
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

    private async Task<GameTurn?> WaitForNextTurnAsync(string gameId, int timeoutSeconds = 30)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
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

    private async Task<GameTurn?> WaitForNextTurnAsync(string turnId, string gameId, int timeoutSeconds = 30)
    {
        var afterObjectId = ObjectId.Parse(turnId);
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

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