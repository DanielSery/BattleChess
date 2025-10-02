using CrownsGuard.Database.Database;
using CrownsGuard.Database.Errors;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CrownsGuard.Database.Lobby;

internal class GameLobbiesCollectionHandler : IGameLobbiesCollectionHandler
{
    private readonly IDatabaseClient _client;
    private readonly ILogger<GameLobbiesCollectionHandler> _logger;

    public GameLobbiesCollectionHandler(IDatabaseClient databaseClient, ILogger<GameLobbiesCollectionHandler> logger)
    {
        _client = databaseClient;
        _logger = logger;
    }

    public async Task<Result<List<PublicLobbyData>>> GetPublicLobbiesAsync(CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Getting public-lobbies");
            return await _client.GameLobbies.Aggregate()
                .Match(l => l.Version == GameVersion.VersionId && string.IsNullOrEmpty(l.JoinedId))
                .Project(doc => new PublicLobbyData
                {
                    Id = doc.Id,
                    LobbyName = doc.LobbyName,
                    Elo = doc.Elo,
                    Locked = (doc.PasswordHash.Length > 0) ? "True" : "False",
                })
                .ToListAsync(cancellationToken: token);
        });
    }

    /// <inheritdoc />
    public async Task<Result<GameLobby>> FindByIdAsync(string lobbyId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Searching for lobby with id: {lobbyId}", lobbyId);
            var filter = Builders<GameLobby>.Filter.Eq(g => g.Id, lobbyId);
            return await _client.GameLobbies.FindSingleResultAsync(filter, cancellationToken: token);
        });
    }

    /// <inheritdoc />
    public async Task<Result<GameLobby>> FindByNameAsync(string lobbyName, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Searching for lobby with name: {lobbyName}", lobbyName);
            var filter = Builders<GameLobby>.Filter.And(
                Builders<GameLobby>.Filter.Eq(g => g.LobbyName, lobbyName),
                Builders<GameLobby>.Filter.Eq(g => g.Version, GameVersion.VersionId)
            );
            return await _client.GameLobbies.FindSingleResultAsync(filter, cancellationToken: token);
        });
    }

    public async Task<Result<GameLobby>> WaitForLobbyAcceptAsync(string lobbyId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Waiting for join request confirmation");
            var streamFilter = Builders<ChangeStreamDocument<GameLobby>>.Filter.And(
                Builders<ChangeStreamDocument<GameLobby>>.Filter.Or(
                    Builders<ChangeStreamDocument<GameLobby>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Update),
                    Builders<ChangeStreamDocument<GameLobby>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Delete)),
                Builders<ChangeStreamDocument<GameLobby>>.Filter.Eq(cs => cs.FullDocument.Id, lobbyId)
            );
            using var streamCursor = await _client.GameLobbies.CreateChangeStreamCursorAsync(streamFilter, cancellationToken: token);
            var streamResult = streamCursor.WaitForUpdateAsync(cancellationToken: token);
        
            var filter = Builders<GameLobby>.Filter.Eq(g => g.Id, lobbyId);
            var foundResult = await _client.GameLobbies.FindSingleResultAsync(filter, cancellationToken: token);
            if (foundResult.IsSuccess && foundResult.Value.JoinedId is not null) return foundResult;
            if (foundResult.HasError<NoResultsFoundError>()) return foundResult;

            return await streamResult;
        });
    }

    public async Task<Result> DeleteGameLobbiesAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Deleting Lobbies of game {GameId}", gameId);
            var filter = Builders<GameLobby>.Filter.Eq(gj => gj.Id, gameId);
            var result = await _client.GameLobbies.DeleteManyAsync(filter, cancellationToken: token);
            return result.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to delete lobbies");
        });
    }

    public async Task<Result> UpdateLobbyJoinAsync(string lobbyId, string joinId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Confirming game join of lobby {lobbyId}", lobbyId);
            var filter = Builders<GameLobby>.Filter.And(
                Builders<GameLobby>.Filter.Eq(l => l.Id, lobbyId),
                Builders<GameLobby>.Filter.Eq(l => l.JoinedId, null));
            var update = Builders<GameLobby>.Update.Set(x => x.JoinedId, joinId);
            var result = await _client.GameLobbies.UpdateOneAsync(filter, update, cancellationToken: token);
            return result.IsAcknowledged && result.ModifiedCount > 0 ? Result.Ok() : Result.Fail("Failed to update game confirmation");
        });
    }

    public async Task<Result> InsertAsync(GameLobby game, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Inserting game lobby");
            await _client.GameLobbies.InsertOneAsync(game, cancellationToken: token);
        });
    }

    public Task WatchChangesAsync(
        Func<ChangeStreamDocument<GameLobby>, Task> onLobbyChange,
        CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            var streamFilter = Builders<ChangeStreamDocument<GameLobby>>.Filter.Or(
                Builders<ChangeStreamDocument<GameLobby>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Insert),
                Builders<ChangeStreamDocument<GameLobby>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Replace),
                Builders<ChangeStreamDocument<GameLobby>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Update),
                Builders<ChangeStreamDocument<GameLobby>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Delete));
            var streamPipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameLobby>>().Match(streamFilter);

            using var cursor = await _client.GameLobbies.WatchAsync(streamPipeline, cancellationToken: cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                var moveResult = await cursor.MoveNextAsync(cancellationToken);
                if (!moveResult) continue;

                foreach (var change in cursor.Current)
                {
                    await onLobbyChange.Invoke(change);
                }
            }
        }, cancellationToken);
    }
}