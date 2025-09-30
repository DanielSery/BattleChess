using CrownsGuard.Database.Database;
using CrownsGuard.Database;
using CrownsGuard.Database.Errors;
using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Database.Ranked;

internal class RankedGameJoinsCollectionHandler : IRankedGameJoinsCollectionHandler
{
    private readonly IDatabaseClient _client;

    public RankedGameJoinsCollectionHandler(IDatabaseClient databaseClient)
    {
        _client = databaseClient;
    }

    public async Task<Result> InsertAsync(RankedGameJoin gameJoin, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Creating join game: {gameJoin.GameId}");
            await _client.RankedGameJoins.InsertOneAsync(gameJoin, cancellationToken: cancellationToken);
            Console.WriteLine($"Created join request with id: {gameJoin.GameId}");
            return Result.Ok();
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled")
                .WithError(CancelledError.Instance);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to create join request with id: {gameJoin.GameId}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> DeleteGameJoinsAsync(string gameId, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Deleting RankedJoins");
            var filter = Builders<RankedGameJoin>.Filter.Eq(gj => gj.GameId, gameId);
            var result = await _client.RankedGameJoins.DeleteManyAsync(filter, cancellationToken: cancellationToken);
            Console.WriteLine($"Deleted RankedJoins: {result.DeletedCount}");
            return result.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to delete RankedJoins");
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled")
                .WithError(CancelledError.Instance);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete RankedJoins, e: {ex}");
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result<RankedGameJoin>> WaitForGameJoinAsync(string gameId, CancellationToken cancellationToken)
    {
        try
        {
            var joinPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RankedGameJoin>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Insert &&
                                 change.FullDocument.GameId == gameId);

            using var cursor = await _client.RankedGameJoins.WatchAsync(joinPipeline, cancellationToken: cancellationToken);

            var filter = Builders<RankedGameJoin>.Filter.Eq(g => g.GameId, gameId);
            var foundGames = await _client.RankedGameJoins.FindAsync(filter, cancellationToken: cancellationToken);
            var foundGame = await foundGames.FirstOrDefaultAsync(cancellationToken);
            if (foundGame is not null)
            {
                return foundGame;
            }

            while (await cursor.MoveNextAsync(cancellationToken))
            {
                foreach (var change in cursor.Current)
                {
                    return change.FullDocument;
                }
            }

            return Result.Fail("Failed to get RankedGameJoin");
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled")
                .WithError(CancelledError.Instance);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to get RankedGameJoins, e: {e}");
            return Result.Fail(e.Message);
        }
    }
}