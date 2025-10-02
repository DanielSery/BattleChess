using System.Diagnostics.CodeAnalysis;
using CrownsGuard.Database.Database;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CrownsGuard.Database.Ranked;

[SuppressMessage("ReSharper", "PossiblyMistakenUseOfCancellationToken")]
internal class RankedGameJoinsCollectionHandler : IRankedGameJoinsCollectionHandler
{
    private readonly IDatabaseClient _client;
    private readonly ILogger<RankedGameJoinsCollectionHandler> _logger;

    public RankedGameJoinsCollectionHandler(IDatabaseClient databaseClient, ILogger<RankedGameJoinsCollectionHandler> logger)
    {
        _client = databaseClient;
        _logger = logger;
    }

    public async Task<Result> InsertAsync(RankedGameJoin gameJoin, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Creating join game: {gameId}", gameJoin.GameId);
            await _client.RankedGameJoins.InsertOneAsync(gameJoin, cancellationToken: linkedSource.Token);
            
        }, nameof(InsertAsync), _logger, cancellationToken);
    }

    public async Task<Result> DeleteGameJoinsAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Deleting RankedJoins");
            var filter = Builders<RankedGameJoin>.Filter.Eq(gj => gj.GameId, gameId);
            var result = await _client.RankedGameJoins.DeleteManyAsync(filter, cancellationToken: linkedSource.Token);
            return result.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to delete RankedJoins");
            
        }, nameof(DeleteGameJoinsAsync), _logger, cancellationToken);
    }

    public async Task<Result<RankedGameJoin>> WaitForGameJoinAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Waiting for ranked join request for game: {gameId}", gameId);

            var streamFilter = Builders<ChangeStreamDocument<RankedGameJoin>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId);
            using var streamCursor = await _client.RankedGameJoins.CreateChangeStreamCursorAsync(streamFilter, cancellationToken: linkedSource.Token);
            var streamResult = streamCursor.WaitForAddAsync(cancellationToken: linkedSource.Token);
        
            var filter = Builders<RankedGameJoin>.Filter.Eq(g => g.GameId, gameId);
            var foundResult = await _client.RankedGameJoins.FindSingleResultAsync(filter, cancellationToken: linkedSource.Token);
            if (foundResult.IsSuccess) return foundResult;

            return await streamResult;
            
        }, nameof(WaitForGameJoinAsync), _logger, cancellationToken);
    }
}