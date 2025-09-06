using CrownsGuard.Multiplayer.Tables;
using FluentResults;

namespace CrownsGuard.Multiplayer.DatabaseAccess;

public interface IGameTurnsCollectionHandler
{
    Task<Result> InsertGameTurn(GameTurn gameTurn, CancellationToken cancellationToken);

    Task<Result> RemoveTurnsOlderThan(DateTime keepFrom, CancellationToken cancellationToken);

    Task<Result<GameTurn>> WaitForFirstTurnAsync(string gameId, TimeSpan timeout);

    Task<Result<GameTurn>> WaitForNextTurnAsync(string turnId, string gameId, TimeSpan timeout);
}