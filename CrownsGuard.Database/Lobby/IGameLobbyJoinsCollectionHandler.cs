using FluentResults;

namespace CrownsGuard.Database.Lobby;

public interface IGameLobbyJoinsCollectionHandler
{
    Task<Result> InsertLobbyJoinAsync(GameLobbyJoin lobbyJoin, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<GameLobbyJoin>> WaitForLobbyJoinAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 1000);

    Task<Result> DeleteGameJoinsAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120);
}