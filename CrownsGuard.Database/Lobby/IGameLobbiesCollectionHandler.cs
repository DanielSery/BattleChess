using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Database.Lobby;

public interface IGameLobbiesCollectionHandler
{
    Task<Result<List<PublicLobbyData>>> GetPublicLobbiesAsync(CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<GameLobby>> FindLobbyByNameAsync(string lobbyName, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<GameLobby>> FindLobbyByIdAsync(string lobbyId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<GameLobby>> WaitForLobbyAcceptAsync(string lobbyId, CancellationToken cancellationToken, int timeoutSeconds = 1_000);

    Task<Result> DeleteGameLobbiesAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result> UpdateLobbyJoinAsync(string lobbyId, string joinId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result> InsertLobbyAsync(GameLobby game, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task WatchChangesAsync(Func<ChangeStreamDocument<GameLobby>, Task> onLobbyChange, CancellationToken cancellationToken);
}