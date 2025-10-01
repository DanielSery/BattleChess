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
    public async Task<Result> InsertAsync(GameTurn gameTurn, CancellationToken cancellationToken, int timeoutSeconds = 120)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(timeoutSeconds);
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Inserting game turn {GameTurnId} for game {GameId}", gameTurn.Id, gameTurn.GameId);

            await _client.GameTurns.InsertOneAsync(gameTurn, cancellationToken: linkedSource.Token);

            _logger.LogInformation("Successfully inserted game turn {GameTurnId}", gameTurn.Id);
        }, nameof(InsertAsync), _logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> RemoveOlderThanAsync(DateTime time, CancellationToken cancellationToken, int timeoutSeconds = 120)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(timeoutSeconds);
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Deleting game turns older than {Time}", time);

            var filter = Builders<GameTurn>.Filter.Lte(gt => gt.CreatedAt, time);
            var result = await _client.GameTurns.DeleteManyAsync(filter, cancellationToken: linkedSource.Token);

            _logger.LogInformation("Successfully deleted {DeletedCount} game turns older than {Time}", result.DeletedCount, time);
        }, nameof(RemoveOlderThanAsync), _logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<GameTurn>> WaitForFirstTurnAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(timeoutSeconds);
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Waiting for first turn for game {GameId} with timeout {Timeout}", gameId, timeoutSeconds);
            var turn = await WaitForFirstTurnInnerAsync(gameId, linkedSource.Token);
            _logger.LogInformation("Received first turn {TurnId} for game {GameId} via change stream", turn.Id, gameId);

            return turn;
        }, nameof(WaitForFirstTurnAsync), _logger, cancellationToken);
    }

    /// <summary>
    /// Waits for a turn via MongoDB change stream
    /// </summary>
    private async Task<GameTurn> WaitForFirstTurnInnerAsync(string gameId, CancellationToken cancellationToken)
    {
        var streamFilter = Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId);
        var streamResult = _client.GameTurns.WaitForAddAsync(streamFilter, cancellationToken: cancellationToken);
        
        var filter = Builders<GameTurn>.Filter.Eq(gt => gt.GameId, gameId);
        var foundResult = await _client.GameTurns.FindSingleResultAsync(filter, cancellationToken: cancellationToken);
        if (foundResult.IsSuccess) return foundResult.Value;

        return await streamResult;
    }

    /// <inheritdoc />
    public async Task<Result<GameTurn>> WaitForNextTurnAsync(string turnId, string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(timeoutSeconds);
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Waiting for next turn after {TurnId} for game {GameId} with timeout {Timeout}", turnId, gameId, timeoutSeconds);
            var turn = await WaitForNextTurnInnerAsync(gameId, turnId, linkedSource.Token);
            _logger.LogInformation("Received next turn {TurnId} for game {GameId} via change stream", turn.Id, gameId);

            return turn;
        }, nameof(WaitForNextTurnAsync), _logger, cancellationToken);
    }

    /// <summary>
    /// Waits for the next turn via MongoDB change stream after the specified ObjectId
    /// </summary>
    private async Task<GameTurn> WaitForNextTurnInnerAsync(string gameId, string afterTurnId, CancellationToken cancellationToken)
    {
        var streamFilter = Builders<ChangeStreamDocument<GameTurn>>.Filter.And(
            Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId),
            Builders<ChangeStreamDocument<GameTurn>>.Filter.Gt(cs => cs.FullDocument.Id, afterTurnId)
        );
        var streamResult = _client.GameTurns.WaitForAddAsync(streamFilter, cancellationToken: cancellationToken);
        
        var filter = Builders<GameTurn>.Filter.And(
            Builders<GameTurn>.Filter.Eq(gt => gt.GameId, gameId),
            Builders<GameTurn>.Filter.Gt(gt => gt.Id, afterTurnId));
        var foundResult = await _client.GameTurns.FindSingleResultAsync(filter, cancellationToken: cancellationToken);
        if (foundResult.IsSuccess) return foundResult.Value;

        return await streamResult;
    }
}