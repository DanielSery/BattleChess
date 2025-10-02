using System.Diagnostics.CodeAnalysis;
using CrownsGuard.Database.Database;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CrownsGuard.Database.Lobby;

[SuppressMessage("ReSharper", "PossiblyMistakenUseOfCancellationToken")]
internal class GameLobbyJoinsCollectionHandler : IGameLobbyJoinsCollectionHandler
{
    private readonly IDatabaseClient _client;
    private readonly ILogger<GameLobbyJoinsCollectionHandler> _logger;

    public GameLobbyJoinsCollectionHandler(IDatabaseClient databaseClient, ILogger<GameLobbyJoinsCollectionHandler> logger)
    {
        _client = databaseClient;
        _logger = logger;
    }

    public async Task<Result> InsertAsync(GameLobbyJoin lobbyJoin, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Inserting join request for game: {GameId}", lobbyJoin.GameId);
            await _client.LobbyGameJoins.InsertOneAsync(lobbyJoin, cancellationToken: linkedSource.Token);
            
        }, nameof(InsertAsync), _logger, cancellationToken);
    }

    public async Task<Result<GameLobbyJoin>> WaitForJoinAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Waiting for join request for game: {gameId}", gameId);

            var streamFilter = Builders<ChangeStreamDocument<GameLobbyJoin>>.Filter.Eq(cs => cs.FullDocument.GameId, gameId);
            using var streamCursor = await _client.LobbyGameJoins.CreateChangeStreamCursorAsync(streamFilter, cancellationToken: linkedSource.Token);
            var streamResult = streamCursor.WaitForAddAsync(cancellationToken: linkedSource.Token);
        
            var filter = Builders<GameLobbyJoin>.Filter.Eq(g => g.GameId, gameId);
            var foundResult = await _client.LobbyGameJoins.FindSingleResultAsync(filter, cancellationToken: linkedSource.Token);
            if (foundResult.IsSuccess) return foundResult;

            return await streamResult;
            
        }, nameof(WaitForJoinAsync), _logger, cancellationToken);
    }

    public async Task<Result> DeleteGameJoinsAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Deleting LobbyJoins");

            var filter = Builders<GameLobbyJoin>.Filter.Eq(gj => gj.GameId, gameId);
            var result = await _client.LobbyGameJoins.DeleteManyAsync(filter, cancellationToken: linkedSource.Token);
            return result.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to delete LobbyJoins");
            
        }, nameof(WaitForJoinAsync), _logger, cancellationToken);
    }
}