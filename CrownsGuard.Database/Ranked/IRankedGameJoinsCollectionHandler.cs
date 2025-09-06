using FluentResults;

namespace CrownsGuard.Database.Ranked;

public interface IRankedGameJoinsCollectionHandler
{
    Task<Result> InsertAsync(RankedGameJoin gameJoin, CancellationToken cancellationToken);

    Task<Result> DeleteGameJoinsAsync(string gameId, CancellationToken cancellationToken);

    Task<Result<RankedGameJoin>> WaitForGameJoinAsync(string gameId, CancellationToken cancellationToken);
}