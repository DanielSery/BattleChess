using FluentResults;

namespace CrownsGuard.Database.Lobby;

public interface IGameLobbyJoinsCollectionHandler
{
    Task<Result> InsertAsync(GameLobbyJoin lobbyJoin, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<GameLobbyJoin>> WaitForJoinAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 1000);

    Task<Result> DeleteGameJoinsAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120);
}