using FluentResults;

namespace CrownsGuard.Database.Ranked;

public interface IRankedGameJoinsCollectionHandler
{
    Task<Result> InsertGameJoin(RankedGameJoin gameJoin, CancellationToken cancellationToken);

    Task<Result> DeleteGameJoins(string gameId, CancellationToken cancellationToken);

    Task<Result<RankedGameJoin>> WaitForGameJoin(string gameId, CancellationToken cancellationToken);
}