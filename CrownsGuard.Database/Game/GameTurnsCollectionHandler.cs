using CrownsGuard.Database.Database;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CrownsGuard.Database.Game;

internal class GameTurnsCollectionHandler : IGameTurnsCollectionHandler
{
    private readonly IDatabaseClient _client;

    public GameTurnsCollectionHandler(IDatabaseClient databaseClient)
    {
        _client = databaseClient;
    }

    /// <inheritdoc />
    public async Task<Result> InsertAsync(GameTurn gameTurn, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Inserting game turn {gameTurn.Id}");
            await _client.GameTurns.InsertOneAsync(gameTurn, cancellationToken: cancellationToken);
            Console.WriteLine($"Game turn {gameTurn.Id} inserted");
            return Result.Ok();
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled")
                .WithError(CancelledError.Instance);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to insert game turn: {e}");
            return Result.Fail(e.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result> RemoveOlderThanAsync(DateTime time, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Deleting GameTurns");
            var filter = Builders<GameTurn>.Filter.Lte(gj => gj.CreatedAt, time);
            var result = await _client.GameTurns.DeleteManyAsync(filter, cancellationToken: cancellationToken);
            Console.WriteLine($"Deleted GameTurns: {result.DeletedCount}");
            return Result.Ok();
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled")
                .WithError(CancelledError.Instance);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to remove game turns: {e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<GameTurn>> WaitForFirstTurnAsync(string gameId, TimeSpan timeout)
    {
        try
        {
            var cancellationTokenSource = new CancellationTokenSource(timeout);
            var filter = Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId);

            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameTurn>>()
                .Match(filter);

            using var cursor = await _client.GameTurns.WatchAsync(
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

            return Result.Fail<GameTurn>($"Game turn {gameId} not found");
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled")
                .WithError(CancelledError.Instance);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to wait for first turn: {e}");
            return Result.Fail<GameTurn>(e.Message);
        }
    }

    public async Task<Result<GameTurn>> WaitForNextTurnAsync(string turnId, string gameId, TimeSpan timeout)
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

            using var cursor = await _client.GameTurns.WatchAsync(
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

            return Result.Fail<GameTurn>($"Game turn {turnId} not found");
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled")
                .WithError(CancelledError.Instance);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to wait for next turn: {e}");
            return Result.Fail<GameTurn>(e.Message);
        }
    }
}