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
        var timeoutTokenSource = new CancellationTokenSource(timeout);
        try
        {
            var changeFilter = Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId);
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameTurn>>()
                .Match(changeFilter);

            using var cursor = await _client.GameTurns.WatchAsync(
                pipeline,
                new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
                timeoutTokenSource.Token
            );
            
            var filter = Builders<GameTurn>.Filter.Eq(g => g.GameId, gameId);
            var foundTurns = await _client.GameTurns.FindAsync(filter, cancellationToken: timeoutTokenSource.Token);
            var foundTurn = await foundTurns.FirstOrDefaultAsync(cancellationToken: timeoutTokenSource.Token);
            if (foundTurn is not null)
            {
                return foundTurn;
            }

            while (await cursor.MoveNextAsync(timeoutTokenSource.Token))
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
            if (timeoutTokenSource.IsCancellationRequested)
            {
                return Result.Fail("Operation timeout")
                    .WithError(TimeoutError.Instance);
            }
            
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
        var timeoutTokenSource = new CancellationTokenSource(timeout);
        try
        {
            var afterObjectId = ObjectId.Parse(turnId);
            var changeFilter = Builders<ChangeStreamDocument<GameTurn>>.Filter.And(
                Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId),
                Builders<ChangeStreamDocument<GameTurn>>.Filter.Gt(cs => cs.FullDocument.Id, turnId)
            );

            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameTurn>>()
                .Match(changeFilter);

            using var cursor = await _client.GameTurns.WatchAsync(
                pipeline,
                new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
                timeoutTokenSource.Token
            );
            
            var filter = Builders<GameTurn>.Filter.And(
                Builders<GameTurn>.Filter.Eq(g => g.GameId, gameId),
                Builders<GameTurn>.Filter.Gt(cs => cs.Id, turnId));
            var foundTurns = await _client.GameTurns.FindAsync(filter, cancellationToken: timeoutTokenSource.Token);
            var foundTurn = await foundTurns.FirstOrDefaultAsync(cancellationToken: timeoutTokenSource.Token);
            if (foundTurn is not null)
            {
                return foundTurn;
            }

            while (await cursor.MoveNextAsync(timeoutTokenSource.Token))
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
            if (timeoutTokenSource.IsCancellationRequested)
            {
                return Result.Fail("Operation timeout")
                    .WithError(TimeoutError.Instance);
            }

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