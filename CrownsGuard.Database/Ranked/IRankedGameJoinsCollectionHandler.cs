using FluentResults;

namespace CrownsGuard.Database.Ranked;

public interface IRankedGameJoinsCollectionHandler
{
    Task<Result> InsertAsync(RankedGameJoin gameJoin, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result> DeleteGameJoinsAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<RankedGameJoin>> WaitForGameJoinAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 1000);
}