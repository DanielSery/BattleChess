using FluentResults;

namespace CrownsGuard.Database.Ranked;

public interface IRankedGamesCollectionHandler
{
    Task<Result> ConfirmGameJoinAsync(string gameId, string joinId, CancellationToken cancellationToken);

    Task<Result> DeleteGameSearchAsync(string deletedGameId, CancellationToken cancellationToken);

    Task<Result<RankedGame>> FindForTargetEloAsync(string gameId, short targetElo, int eloDifference, CancellationToken cancellationToken);

    Task<Result<RankedGame>> WaitForAcceptAsync(string joinedGameId, int timeoutSeconds, CancellationToken cancellationToken);

    Task<Result> InsertAsync(RankedGame game, CancellationToken cancellationToken);

    Task<Result<RankedGame>> GetClosestGameSearchAsync(int searchedElo, int maxDifference, CancellationToken cancellationToken);
}