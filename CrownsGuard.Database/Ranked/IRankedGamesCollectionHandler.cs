using FluentResults;

namespace CrownsGuard.Database.Ranked;

public interface IRankedGamesCollectionHandler
{
    Task<Result> ConfirmGameJoinAsync(string gameId, string joinId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result> DeleteGameSearchAsync(string deletedGameId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<RankedGame>> FindForTargetEloAsync(string gameId, short targetElo, int eloDifference, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<RankedGame>> WaitForAcceptAsync(string joinedGameId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result> InsertAsync(RankedGame game, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<RankedGame>> GetClosestGameSearchAsync(int searchedElo, int maxDifference, CancellationToken cancellationToken, int timeoutSeconds = 120);
}