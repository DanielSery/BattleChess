using CrownsGuard.Multiplayer.Tables;
using FluentResults;

namespace CrownsGuard.Multiplayer.DatabaseAccess;

public interface IRankedGameJoinsCollectionHandler
{
    Task<Result> InsertGameJoin(RankedGameJoin gameJoin, CancellationToken cancellationToken);

    Task<Result> DeleteGameJoins(string gameId, CancellationToken cancellationToken);

    Task<Result<RankedGameJoin>> WaitForGameJoin(string gameId, CancellationToken cancellationToken);
}