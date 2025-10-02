using CrownsGuard.Database.Database;
using CrownsGuard.Database.Errors;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CrownsGuard.Database.Ranked;

internal class RankedGamesCollectionHandler : IRankedGamesCollectionHandler
{
    private readonly IDatabaseClient _client;
    private readonly ILogger<RankedGamesCollectionHandler> _logger;

    public RankedGamesCollectionHandler(IDatabaseClient databaseClient, ILogger<RankedGamesCollectionHandler> logger)
    {
        _client = databaseClient;
        _logger = logger;
    }

    public async Task<Result> DeleteGameSearchAsync(string deletedGameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Deleting game search");
            var gameSearchFilter = Builders<RankedGame>.Filter.Eq(l => l.Id, deletedGameId);
            var result = await _client.RankedGames.DeleteManyAsync(gameSearchFilter, token);
            return result.ToResult("Failed to delete game joins");
        });
    }

    public async Task<Result<RankedGame>> FindGameForTargetEloAsync(string gameId, short targetElo, int eloDifference, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            var fromElo = targetElo - eloDifference;
            var toElo = targetElo + eloDifference;
            
            _logger.LogInformation("Waiting for ranked join request for game: {gameId}", gameId);
            var streamFilter = Builders<ChangeStreamDocument<RankedGame>>.Filter.And(
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Insert),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Gt(cs => cs.FullDocument.Id, gameId),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Eq(cs => cs.FullDocument.Version, GameVersion.VersionId),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Eq(cs => cs.FullDocument.JoinedId, null),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Gte(cs => cs.FullDocument.Elo, fromElo),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Lte(cs => cs.FullDocument.Elo, toElo));
            using var streamCursor = await _client.RankedGames.WatchAsync(streamFilter, cancellationToken: token);
        
            var filter = Builders<RankedGame>.Filter.And(
                Builders<RankedGame>.Filter.Gt(g => g.Id, gameId),
                Builders<RankedGame>.Filter.Eq(g => g.Version, GameVersion.VersionId),
                Builders<RankedGame>.Filter.Eq(g => g.JoinedId, null),
                Builders<RankedGame>.Filter.Gte(g => g.Elo, fromElo),
                Builders<RankedGame>.Filter.Lte(g => g.Elo, toElo));
            var foundResult = await _client.RankedGames.FindFirstResultAsync(filter, cancellationToken: token);
            
            if (foundResult.IsSuccess) return foundResult;
            return await streamCursor.WaitForAddAsync(cancellationToken: token);
        });
    }

    public async Task<Result<RankedGame>> WaitForGameConfirmationAsync(string joinedGameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Getting game with id: {joinedGameId}", joinedGameId);
            var streamFilter = Builders<ChangeStreamDocument<RankedGame>>.Filter.And(
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Or(
                    Builders<ChangeStreamDocument<RankedGame>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Update),
                    Builders<ChangeStreamDocument<RankedGame>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Delete)),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Eq(cs => cs.FullDocument.Id, joinedGameId));
            using var streamCursor = await _client.RankedGames.WatchAsync(streamFilter, cancellationToken: token);
        
            var filter = Builders<RankedGame>.Filter.Eq(g => g.Id, joinedGameId);
            var foundResult = await _client.RankedGames.FindSingleResultAsync(filter, cancellationToken: token);
            
            if (foundResult.IsSuccess && foundResult.Value.JoinedId is not null) return foundResult;
            if (foundResult.HasError<NoResultsFoundError>()) return foundResult;
            return await streamCursor.WaitForUpdateAsync(cancellationToken: token);
        });
    }

    public async Task<Result> ConfirmGameJoinAsync(string gameId, string joinId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Confirming game join");
            var filter = Builders<RankedGame>.Filter.Eq(l => l.Id, gameId);
            var update = Builders<RankedGame>.Update.Set(x => x.JoinedId, joinId);
            var result = await _client.RankedGames.UpdateOneAsync(filter, update, cancellationToken: token);
            return result.ToResult("Failed to delete confirm game join");
        });
    }

    public async Task<Result> InsertGameAsync(RankedGame game, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Creating game request");
            await _client.RankedGames.InsertOneAsync(game, cancellationToken: token);
        });
    }

    public async Task<Result<RankedGame>> GetClosestGameSearchAsync(int searchedElo, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Searching for ranked game near elo: {searchedElo}", searchedElo);
            return await _client.RankedGames.Aggregate()
                .Match(l => l.Version == GameVersion.VersionId && string.IsNullOrEmpty(l.JoinedId))
                .Project(lobby => new
                {
                    Lobby = lobby,
                    EloDifference = Math.Abs(lobby.Elo - searchedElo)
                })
                .SortBy(x => x.EloDifference)
                .Limit(1)
                .Project(x => x.Lobby)
                .FirstAsync(cancellationToken: token);
        });
    }
}
