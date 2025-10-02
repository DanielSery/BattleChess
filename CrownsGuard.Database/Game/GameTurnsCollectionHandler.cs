using CrownsGuard.Database.Database;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CrownsGuard.Database.Game;

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
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Inserting game turn {GameTurnId} for game {GameId}", gameTurn.Id, gameTurn.GameId);
            await _client.GameTurns.InsertOneAsync(gameTurn, cancellationToken: token);
        });
    }

    /// <inheritdoc />
    public async Task<Result> RemoveTurnsOlderThanAsync(DateTime time, CancellationToken cancellationToken, int timeoutSeconds = 120)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Deleting game turns older than {Time}", time);
            var filter = Builders<GameTurn>.Filter.Lte(gt => gt.CreatedAt, time);
            var result = await _client.GameTurns.DeleteManyAsync(filter, cancellationToken: token);
            return result.ToResult("Failed to delete turns");
        });
    }

    /// <inheritdoc />
    public async Task<Result<GameTurn>> WaitForFirstTurnAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Waiting for first turn for game {GameId}", gameId);
            var streamFilter = Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId);
            using var streamCursor = await _client.GameTurns.WatchAsync(streamFilter, cancellationToken: token);
        
            var filter = Builders<GameTurn>.Filter.Eq(gt => gt.GameId, gameId);
            var foundResult = await _client.GameTurns.FindSingleResultAsync(filter, cancellationToken: token);
            
            if (foundResult.IsSuccess) return foundResult.Value;
            return await streamCursor.WaitForAddAsync(cancellationToken: token);
        });
    }

    /// <inheritdoc />
    public async Task<Result<GameTurn>> WaitForNextTurnAsync(string turnId, string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Waiting for next turn after {TurnId} for game {GameId}", turnId, gameId);
            var streamFilter = Builders<ChangeStreamDocument<GameTurn>>.Filter.And(
                Builders<ChangeStreamDocument<GameTurn>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId),
                Builders<ChangeStreamDocument<GameTurn>>.Filter.Gt(cs => cs.FullDocument.Id, turnId)
            );
            using var streamCursor = await _client.GameTurns.WatchAsync(streamFilter, cancellationToken: token);
        
            var filter = Builders<GameTurn>.Filter.And(
                Builders<GameTurn>.Filter.Eq(gt => gt.GameId, gameId),
                Builders<GameTurn>.Filter.Gt(gt => gt.Id, turnId));
            var foundResult = await _client.GameTurns.FindSingleResultAsync(filter, cancellationToken: token);
            
            if (foundResult.IsSuccess) return foundResult.Value;
            return await streamCursor.WaitForAddAsync(cancellationToken: token);
        });
    }
}