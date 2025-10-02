using System.Diagnostics.CodeAnalysis;
using CrownsGuard.Database.Database;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CrownsGuard.Database.Game;

[SuppressMessage("ReSharper", "PossiblyMistakenUseOfCancellationToken")]
internal class GameTurnsCollectionHandler : IGameTurnsCollectionHandler
{
    private readonly IDatabaseClient _client;
    private readonly ILogger<GameTurnsCollectionHandler> _logger;

    public GameTurnsCollectionHandler(IDatabaseClient databaseClient, ILogger<GameTurnsCollectionHandler> logger)
    {
        _client = databaseClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> InsertTurnAsync(GameTurn gameTurn, CancellationToken cancellationToken, int timeoutSeconds = 120)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Inserting game turn {GameTurnId} for game {GameId}", gameTurn.Id, gameTurn.GameId);
            await _client.GameTurns.InsertOneAsync(gameTurn, cancellationToken: linkedSource.Token);
            
        }, nameof(InsertTurnAsync), _logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> RemoveTurnsOlderThanAsync(DateTime time, CancellationToken cancellationToken, int timeoutSeconds = 120)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Deleting game turns older than {Time}", time);

            var filter = Builders<GameTurn>.Filter.Lte(gt => gt.CreatedAt, time);
            var result = await _client.GameTurns.DeleteManyAsync(filter, cancellationToken: linkedSource.Token);

            _logger.LogInformation("Successfully deleted {DeletedCount} game turns older than {Time}", result.DeletedCount, time);
            
        }, nameof(RemoveTurnsOlderThanAsync), _logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<GameTurn>> WaitForFirstTurnAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Waiting for first turn for game {GameId}", gameId);
            
            var streamFilter = Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId);
            using var streamCursor = await _client.GameTurns.CreateChangeStreamCursorAsync(streamFilter, cancellationToken: linkedSource.Token);
            var streamResult = streamCursor.WaitForAddAsync(cancellationToken: linkedSource.Token);
        
            var filter = Builders<GameTurn>.Filter.Eq(gt => gt.GameId, gameId);
            var foundResult = await _client.GameTurns.FindSingleResultAsync(filter, cancellationToken: linkedSource.Token);
            if (foundResult.IsSuccess) return foundResult.Value;

            return await streamResult;
        }, nameof(WaitForFirstTurnAsync), _logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<GameTurn>> WaitForNextTurnAsync(string turnId, string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Waiting for next turn after {TurnId} for game {GameId}", turnId, gameId);
            
            var streamFilter = Builders<ChangeStreamDocument<GameTurn>>.Filter.And(
                Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId),
                Builders<ChangeStreamDocument<GameTurn>>.Filter.Gt(cs => cs.FullDocument.Id, turnId)
            );
            using var streamCursor = await _client.GameTurns.CreateChangeStreamCursorAsync(streamFilter, cancellationToken: linkedSource.Token);
            var streamResult = streamCursor.WaitForAddAsync(cancellationToken: linkedSource.Token);
        
            var filter = Builders<GameTurn>.Filter.And(
                Builders<GameTurn>.Filter.Eq(gt => gt.GameId, gameId),
                Builders<GameTurn>.Filter.Gt(gt => gt.Id, turnId));
            var foundResult = await _client.GameTurns.FindSingleResultAsync(filter, cancellationToken: linkedSource.Token);
            if (foundResult.IsSuccess) return foundResult.Value;

            return await streamResult;

        }, nameof(WaitForNextTurnAsync), _logger, cancellationToken);
    }
}