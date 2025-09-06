using CrownsGuard.Multiplayer.Tables;
using FluentResults;

namespace CrownsGuard.Multiplayer;

public interface IGameLobbyJoinsCollectionHandler
{
    Task<Result> InsertGameJoin(GameLobbyJoin lobbyJoin, CancellationToken cancellationToken);

    Task<Result<GameLobbyJoin>> WaitForGameJoinAsync(string gameId, CancellationToken cancellationToken);

    Task<Result> DeleteGameJoins(string gameId, CancellationToken cancellationToken);
}