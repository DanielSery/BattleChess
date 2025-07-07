using BattleChess3.Game.Board;
using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

internal sealed class MultiplayerGameService : IMultiplayerGameService
{
    private string? _gameId;
    
    private readonly IMultiplayerScheduler _scheduler;
    
    private readonly IMongoCollection<GameTurn> _gameTurnsCollection;

    public MultiplayerGameService(
        IMultiplayerScheduler scheduler)
    {
        _scheduler = scheduler;
        
        var client = new MongoClient(DbSecrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _gameTurnsCollection = database.GetCollection<GameTurn>("GameTurns");
    }

    public event EventHandler<(Position, Position)>? RequestPlayMove;

    public string? CurrentGameId => _gameId;

    /// <inheritdoc />
    public void StartGame(string? gameId)
    {
        _gameId = gameId;
    }

    public Task<Result> PlayedMove(Position from, Position to)
    {
        lock (_scheduler.SyncLock)
        {
            if (_gameId is null)
                return Task.FromResult(Result.Fail("Not in game"));
        
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Creating game turn: {from} to {to}");
                    var gameTurn = new GameTurn()
                    {
                        GameId = _gameId,
                        FromIndex = (byte)from.Index,
                        ToIndex = (byte)to.Index
                    };
                    await _gameTurnsCollection.InsertOneAsync(gameTurn);
                    Console.WriteLine($"Created game turn: {from} to {to}");

                    await HandleHisTurn(gameTurn.Id);
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

    public async Task HandleHisTurn(string? turnId = null)
    {
        if (_gameId is null) throw new ArgumentNullException(nameof(turnId));
        
        Console.WriteLine($"Waiting for his turn with id greater than: {turnId}");
        var hisTurn = turnId is null
            ? await WaitForNextTurnAsync(_gameId)
            : await WaitForNextTurnAsync(turnId, _gameId);
        Console.WriteLine($"Found his turn with id: {hisTurn.Id}");

        RequestPlayMove?.Invoke(this, new ValueTuple<Position, Position>(
            GetPositionOfOppositePlayer(hisTurn.FromIndex),
            GetPositionOfOppositePlayer(hisTurn.ToIndex)));
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

    public Task<Result> DeleteGame(string gameId)
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