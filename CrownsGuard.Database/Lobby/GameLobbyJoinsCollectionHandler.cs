using CrownsGuard.Database.Database;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CrownsGuard.Database.Lobby;

internal class GameLobbyJoinsCollectionHandler : IGameLobbyJoinsCollectionHandler
{
    private readonly IDatabaseClient _client;
    private readonly ILogger<GameLobbyJoinsCollectionHandler> _logger;

    public GameLobbyJoinsCollectionHandler(IDatabaseClient databaseClient, ILogger<GameLobbyJoinsCollectionHandler> logger)
    {
        _client = databaseClient;
        _logger = logger;
    }

    public async Task<Result> InsertLobbyJoinAsync(GameLobbyJoin lobbyJoin, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Inserting join request for game: {GameId}", lobbyJoin.GameId);
            await _client.LobbyGameJoins.InsertOneAsync(lobbyJoin, cancellationToken: token);
        });
    }

    public async Task<Result<GameLobbyJoin>> WaitForLobbyJoinAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Waiting for join request for game: {gameId}", gameId);
            var streamFilter = Builders<ChangeStreamDocument<GameLobbyJoin>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId);
            using var streamCursor = await _client.LobbyGameJoins.WatchAsync(streamFilter, cancellationToken: token);
        
            var filter = Builders<GameLobbyJoin>.Filter.Eq(g => g.GameId, gameId);
            var foundResult = await _client.LobbyGameJoins.FindSingleResultAsync(filter, cancellationToken: token);
            
            if (foundResult.IsSuccess) return foundResult;
            return await streamCursor.WaitForAddAsync(cancellationToken: token);
        });
    }

    public async Task<Result> DeleteGameJoinsAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Deleting LobbyJoins");
            var filter = Builders<GameLobbyJoin>.Filter.Eq(gj => gj.GameId, gameId);
            var result = await _client.LobbyGameJoins.DeleteManyAsync(filter, cancellationToken: token);
            return result.ToResult("Failed to delete LobbyJoins");
        });
    }
}