using CrownsGuard.Database.Database;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CrownsGuard.Database.Ranked;

internal class RankedGameJoinsCollectionHandler : IRankedGameJoinsCollectionHandler
{
    private readonly IDatabaseClient _client;
    private readonly ILogger<RankedGameJoinsCollectionHandler> _logger;

    public RankedGameJoinsCollectionHandler(IDatabaseClient databaseClient, ILogger<RankedGameJoinsCollectionHandler> logger)
    {
        _client = databaseClient;
        _logger = logger;
    }

    public async Task<Result> InsertGameJoinAsync(RankedGameJoin gameJoin, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Creating join game: {gameId}", gameJoin.GameId);
            await _client.RankedGameJoins.InsertOneAsync(gameJoin, cancellationToken: token);
        });
    }

    public async Task<Result> DeleteGameJoinsAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Deleting RankedJoins");
            var filter = Builders<RankedGameJoin>.Filter.Eq(gj => gj.GameId, gameId);
            var result = await _client.RankedGameJoins.DeleteManyAsync(filter, cancellationToken: token);
            return result.ToResult("Failed to delete RankedJoins");
        });
    }

    public async Task<Result<RankedGameJoin>> WaitForGameJoinAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Waiting for ranked join request for game: {gameId}", gameId);
            var streamFilter = Builders<ChangeStreamDocument<RankedGameJoin>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId);
            using var streamCursor = await _client.RankedGameJoins.WatchAsync(streamFilter, cancellationToken: token);
        
            var filter = Builders<RankedGameJoin>.Filter.Eq(g => g.GameId, gameId);
            var foundResult = await _client.RankedGameJoins.FindSingleResultAsync(filter, cancellationToken: token);
            
            if (foundResult.IsSuccess) return foundResult;
            return await streamCursor.WaitForAddAsync(cancellationToken: token);
        });
    }
}