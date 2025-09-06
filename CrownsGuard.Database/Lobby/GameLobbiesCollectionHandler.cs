using CrownsGuard.Database.Database;
using CrownsGuard.Database.Utilities;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CrownsGuard.Database.Lobby;

internal class GameLobbiesCollectionHandler : IGameLobbiesCollectionHandler
{
    private readonly IDatabaseClient _client;

    public GameLobbiesCollectionHandler(IDatabaseClient databaseClient)
    {
        _client = databaseClient;
    }

    public async Task<Result<List<PublicLobbyData>>> GetPublicLobbiesAsync(CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Getting public-lobbies");
            var publicLobbies = await _client.GameLobbies.Aggregate()
                .Match(l => l.Version == GameVersion.VersionId && string.IsNullOrEmpty(l.JoinedId))
                .Project(doc => new PublicLobbyData
                {
                    Id = doc.Id,
                    LobbyName = doc.LobbyName,
                    Elo = doc.Elo,
                    Locked = (doc.PasswordHash.Length > 0) ? "True" : "False",
                })
                .ToListAsync(cancellationToken: cancellationToken);
            Console.WriteLine("Found public-lobbies");
            return publicLobbies;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to get public-lobbies: {e}");
            return Result.Fail<List<PublicLobbyData>>(e.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<GameLobby>> FindByIdAsync(string lobbyId, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Searching for lobby with id: {lobbyId}");
            var filter = Builders<GameLobby>.Filter.Eq(g => g.Id, lobbyId);
            var foundGames = await _client.GameLobbies.FindAsync(filter, cancellationToken: cancellationToken);
            var foundGameResult = await foundGames.SingleResultAsync(cancellationToken: cancellationToken);
            Console.WriteLine($"Found lobby with id: {lobbyId}");
            return foundGameResult;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to get game-lobby with id {lobbyId}: {e}");
            return Result.Fail<GameLobby>(e.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<GameLobby>> FindByNameAsync(string lobbyName, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Searching for lobby with name: {lobbyName}");
            var filter = Builders<GameLobby>.Filter.And(
                Builders<GameLobby>.Filter.Eq(g => g.LobbyName, lobbyName),
                Builders<GameLobby>.Filter.Eq(g => g.Version, GameVersion.VersionId)
            );
            var foundGames = await _client.GameLobbies.FindAsync(filter, cancellationToken: cancellationToken);
            var foundGameResult = await foundGames.SingleResultAsync(cancellationToken: cancellationToken);
            Console.WriteLine($"Found lobby with name: {lobbyName}");
            return foundGameResult;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to get game-lobby with name {lobbyName}: {e}");
            return Result.Fail<GameLobby>(e.Message);
        }
    }

    public async Task<Result<GameLobby>> WaitForLobbyAcceptAsync(string lobbyId, CancellationToken cancellationToken)
    {
        try
        {
            using var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using var compositeTimeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameLobby>>()
                .Match(change =>
                    (change.OperationType == ChangeStreamOperationType.Update ||
                     change.OperationType == ChangeStreamOperationType.Delete) &&
                    change.DocumentKey["_id"] == ObjectId.Parse(lobbyId));

            Console.WriteLine("Waiting for join request confirmation");
            using var cursor = await _client.GameLobbies.WatchAsync(
                pipeline,
                new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
                compositeTimeoutTokenSource.Token
            );

            while (await cursor.MoveNextAsync(compositeTimeoutTokenSource.Token))
            {
                foreach (var change in cursor.Current)
                {
                    if (change.FullDocument.Id != lobbyId)
                        continue;

                    if (change.OperationType == ChangeStreamOperationType.Delete)
                        return Result.Fail("Lobby deleted");

                    Console.WriteLine("Join request confirmed");
                    return change.FullDocument;
                }
            }

            return Result.Fail<GameLobby>("Lobby not found");
        }
        catch (OperationCanceledException)
        {
            return Result.Fail<GameLobby>("Operation cancelled");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to wait for accept e:{e}");
            return Result.Fail<GameLobby>(e.Message);
        }
    }

    public async Task<Result> DeleteGameLobbiesAsync(string gameId, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Deleting Lobbies");
            var filter = Builders<GameLobby>.Filter.Eq(gj => gj.Id, gameId);
            var result = await _client.GameLobbies.DeleteManyAsync(filter, cancellationToken: cancellationToken);
            Console.WriteLine($"Deleted Lobbies: {result.DeletedCount}");
            return result.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to delete lobbies");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete lobbies: {ex}");
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result> UpdateLobbyJoinAsync(string lobbyId, string joinId, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Confirming game join");
            var filter = Builders<GameLobby>.Filter.Eq(l => l.Id, lobbyId);
            var update = Builders<GameLobby>.Update.Set(x => x.JoinedId, joinId);
            var result = await _client.GameLobbies.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            if (!result.IsAcknowledged) return Result.Fail("Failed to update game confirmation");
            Console.WriteLine($"Confirmed game join for request: {joinId}");
            return Result.Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to update game join for request: {e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> InsertAsync(GameLobby game, CancellationToken cancellationToken)
    {
        try
        {
            game.Version = GameVersion.VersionId;
            Console.WriteLine("Inserting game lobby");
            await _client.GameLobbies.InsertOneAsync(game, cancellationToken: cancellationToken);
            Console.WriteLine("Inserted game lobby");
            return Result.Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to insert game lobby: {e}");
            return Result.Fail(e.Message);
        }
    }

    public Task WatchChangesAsync(
        Func<ChangeStreamDocument<GameLobby>, Task> onLobbyChange,
        CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameLobby>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Insert ||
                                 change.OperationType == ChangeStreamOperationType.Replace ||
                                 change.OperationType == ChangeStreamOperationType.Update ||
                                 change.OperationType == ChangeStreamOperationType.Delete);

            using var cursor = await _client.GameLobbies.WatchAsync(pipeline, cancellationToken: cancellationToken);
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