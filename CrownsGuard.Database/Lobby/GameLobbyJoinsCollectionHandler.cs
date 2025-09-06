using CrownsGuard.Database.Database;
using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Database.Lobby;

internal class GameLobbyJoinsCollectionHandler : IGameLobbyJoinsCollectionHandler
{
    private readonly IDatabaseClient _client;

    public GameLobbyJoinsCollectionHandler(IDatabaseClient databaseClient)
    {
        _client = databaseClient;
    }

    public async Task<Result> InsertAsync(GameLobbyJoin lobbyJoin, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Inserting join request for game: {lobbyJoin.GameId}");
            await _client.LobbyGameJoins.InsertOneAsync(lobbyJoin, cancellationToken: cancellationToken);
            Console.WriteLine($"Created join request with id: {lobbyJoin.GameId}");
            return Result.Ok();
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled")
                .WithError(CancelledError.Instance);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to add join request for game: {lobbyJoin.GameId}, e: {e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<GameLobbyJoin>> WaitForJoinAsync(string gameId, CancellationToken cancellationToken)
    {
        try
        {
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameLobbyJoin>>()
                .Match(Builders<ChangeStreamDocument<GameLobbyJoin>>.Filter
                    .Eq(cs => cs.FullDocument.GameId, gameId));

            using var cursor = await _client.LobbyGameJoins.WatchAsync(
                pipeline,
                new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
                cancellationToken
            );

            Console.WriteLine($"Waiting for join request for game: {gameId}");
            while (await cursor.MoveNextAsync(cancellationToken))
            {
                foreach (var change in cursor.Current)
                {
                    if (change.FullDocument.GameId == gameId)
                    {
                        Console.WriteLine($"Found join request for game: {gameId}");
                        return change.FullDocument;
                    }
                }
            }

            return Result.Fail<GameLobbyJoin>($"Game join with id: {gameId} not found");
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled")
                .WithError(CancelledError.Instance);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to get join request for game: {gameId}, e: {e}");
            return Result.Fail<GameLobbyJoin>(e.Message);
        }
    }

    public async Task<Result> DeleteGameJoinsAsync(string gameId, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Deleting LobbyJoins");
            var filter = Builders<GameLobbyJoin>.Filter.Eq(gj => gj.GameId, gameId);
            var result = await _client.LobbyGameJoins.DeleteManyAsync(filter, cancellationToken: cancellationToken);
            Console.WriteLine($"Deleted LobbyJoins: {result.DeletedCount}");
            return result.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to delete LobbyJoins");
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled")
                .WithError(CancelledError.Instance);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete LobbyJoins, e: {ex}");
            return Result.Fail(ex.Message);
        }
    }
}