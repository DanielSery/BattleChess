using CrownsGuard.Database.Database;
using CrownsGuard.Database.Utilities;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CrownsGuard.Database.Ranked;

internal class RankedGamesCollectionHandler : IRankedGamesCollectionHandler
{
    private readonly IDatabaseClient _client;

    public RankedGamesCollectionHandler(IDatabaseClient databaseClient)
    {
        _client = databaseClient;
    }

    public async Task<Result> ConfirmGameJoinAsync(string gameId, string joinId, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Confirming game join");
            var filter = Builders<RankedGame>.Filter.Eq(l => l.Id, gameId);
            var update = Builders<RankedGame>.Update.Set(x => x.JoinedId, joinId);

            var result = await _client.RankedGames.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            if (!result.IsAcknowledged)
            {
                return Result.Fail("Failed to update game confirmation");
            }

            Console.WriteLine($"Confirmed game join for request: {joinId}");
            return Result.Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to confirm game join for request: {joinId}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> DeleteGameSearchAsync(string deletedGameId, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Deleting game search");
            var gameSearchFilter = Builders<RankedGame>.Filter.Eq(l => l.Id, deletedGameId);
            var gameSearchDeletion = await _client.RankedGames.DeleteManyAsync(gameSearchFilter, cancellationToken);
            Console.WriteLine($"Deleted game search count: {gameSearchDeletion.DeletedCount}");
            return Result.Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to delete game search: {deletedGameId}, e:{e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<RankedGame>> FindForTargetEloAsync(string gameId, short targetElo, int eloDifference, CancellationToken cancellationToken)
    {
        try
        {
            var gameObjectId = ObjectId.Parse(gameId);
            var searchesPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RankedGame>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Insert &&
                                 change.DocumentKey["_id"] > gameObjectId &&
                                 change.FullDocument.Version == GameVersion.VersionId &&
                                 string.IsNullOrEmpty(change.FullDocument.JoinedId) &&
                                 Math.Abs(change.FullDocument.Elo - targetElo) < eloDifference);

            using var cursor = await _client.RankedGames.WatchAsync(searchesPipeline, cancellationToken: cancellationToken);
            var filter = Builders<RankedGame>.Filter.And(
                Builders<RankedGame>.Filter.Gt(g => g.Id, gameId),
                Builders<RankedGame>.Filter.Eq(g => g.Version, GameVersion.VersionId),
                Builders<RankedGame>.Filter.Eq(g => g.JoinedId, null),
                Builders<RankedGame>.Filter.Where(g => Math.Abs(g.Elo - targetElo) < eloDifference));

            var foundGames = await _client.RankedGames.FindAsync(filter, cancellationToken: cancellationToken);
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

            return Result.Fail<RankedGame>("No game found");
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to find game: {gameId}, e:{e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<RankedGame>> WaitForAcceptAsync(string joinedGameId, int timeoutSeconds, CancellationToken cancellationToken)
    {
        try
        {
            using var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            using var cancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RankedGame>>()
                .Match(change =>
                    (change.OperationType == ChangeStreamOperationType.Update ||
                     change.OperationType == ChangeStreamOperationType.Delete) &&
                    change.DocumentKey["_id"] == ObjectId.Parse(joinedGameId));

            using var cursor = await _client.RankedGames.WatchAsync(
                pipeline,
                new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
                cancellationTokenSource.Token
            );

            Console.WriteLine($"Getting game with id: {joinedGameId}");
            var filter = Builders<RankedGame>.Filter.Eq("Id", joinedGameId);
            var foundGames = await _client.RankedGames.FindAsync(filter, cancellationToken: cancellationTokenSource.Token);
            var foundGame = await foundGames.FirstOrDefaultAsync(cancellationToken: cancellationTokenSource.Token);
            if (foundGame is null)
            {
                return Result.Fail<RankedGame>("No game found");
            }

            if (!string.IsNullOrEmpty(foundGame.JoinedId))
            {
                await cancellationTokenSource.CancelAsync();
                return foundGame;
            }

            Console.WriteLine($"Found game with id: {foundGame.Id}");
            while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
            {
                foreach (var change in cursor.Current)
                {
                    if (change.FullDocument.Id != joinedGameId)
                        continue;

                    if (change.OperationType == ChangeStreamOperationType.Delete)
                        return Result.Fail("Game was deleted");

                    return change.FullDocument;
                }
            }

            return Result.Fail("No game join found");
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to find game join: {joinedGameId}, e:{e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> InsertAsync(RankedGame game, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Creating game request");
            await _client.RankedGames.InsertOneAsync(game, cancellationToken: cancellationToken);
            Console.WriteLine($"Created game request: {game.Id}");
            return Result.Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to create game: {game.Id}, e:{e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<RankedGame>> GetClosestGameSearchAsync(int searchedElo, int maxDifference, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Searching for ranked game with elo: {searchedElo - maxDifference}-{searchedElo + maxDifference}");
            var closestGameSearch = await _client.RankedGames.Aggregate()
                .Match(l => l.Version == GameVersion.VersionId && string.IsNullOrEmpty(l.JoinedId))
                .Project(lobby => new
                {
                    Lobby = lobby,
                    EloDifference = Math.Abs(lobby.Elo - searchedElo)
                })
                .SortBy(x => x.EloDifference)
                .Limit(1)
                .Project(x => x.Lobby)
                .FirstAsync(cancellationToken: cancellationToken);
            Console.WriteLine($"Closest game search elo: {closestGameSearch.Elo}");
            return closestGameSearch;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to get closest game to elo {searchedElo}, e: {e.Message}");
            return Result.Fail(e.Message);
        }
    }
}
