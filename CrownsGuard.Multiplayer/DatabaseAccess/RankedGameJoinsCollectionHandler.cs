using CrownsGuard.Multiplayer.Tables;
using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Multiplayer.DatabaseAccess;

public class RankedGameJoinsCollectionHandler : IRankedGameJoinsCollectionHandler
{
    private readonly IMongoCollection<RankedGameJoin> _gameJoins;

    public RankedGameJoinsCollectionHandler(IDatabaseClient databaseClient)
    {
        _gameJoins = databaseClient.RankedGameJoins!;
    }

    public async Task<Result> InsertGameJoin(RankedGameJoin gameJoin, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Creating join game: {gameJoin.GameId}");
            await _gameJoins.InsertOneAsync(gameJoin, cancellationToken: cancellationToken);
            Console.WriteLine($"Created join request with id: {gameJoin.GameId}");
            return Result.Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to create join request with id: {gameJoin.GameId}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> DeleteGameJoins(string gameId, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Deleting RankedJoins");
            var filter = Builders<RankedGameJoin>.Filter.Eq(gj => gj.GameId, gameId);
            var result = await _gameJoins.DeleteManyAsync(filter, cancellationToken: cancellationToken);
            Console.WriteLine($"Deleted RankedJoins: {result.DeletedCount}");
            return result.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to delete RankedJoins");;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete RankedJoins, e: {ex}");
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result<RankedGameJoin>> WaitForGameJoin(string gameId, CancellationToken cancellationToken)
    {
        try
        {
            var joinPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RankedGameJoin>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Insert &&
                                 change.FullDocument.GameId == gameId);

            using var cursor = await _gameJoins.WatchAsync(joinPipeline, cancellationToken: cancellationToken);

            var filter = Builders<RankedGameJoin>.Filter.Eq(g => g.GameId, gameId);
            var foundGames = await _gameJoins.FindAsync(filter, cancellationToken: cancellationToken);
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
        catch (Exception e)
        {
            Console.WriteLine($"Failed to get RankedGameJoins, e: {e}");
            return Result.Fail(e.Message);
        }
    }
}