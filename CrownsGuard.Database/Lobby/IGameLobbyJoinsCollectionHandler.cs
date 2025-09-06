using FluentResults;

namespace CrownsGuard.Database.Lobby;

public interface IGameLobbyJoinsCollectionHandler
{
    Task<Result> InsertAsync(GameLobbyJoin lobbyJoin, CancellationToken cancellationToken);

    Task<Result<GameLobbyJoin>> WaitForJoinAsync(string gameId, CancellationToken cancellationToken);

    Task<Result> DeleteGameJoinsAsync(string gameId, CancellationToken cancellationToken);
}