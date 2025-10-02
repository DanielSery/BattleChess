using FluentResults;

namespace CrownsGuard.Database.Ranked;

public interface IRankedGamesCollectionHandler
{
    Task<Result> ConfirmGameJoinAsync(string gameId, string joinId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result> DeleteGameSearchAsync(string deletedGameId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<RankedGame>> FindGameForTargetEloAsync(string gameId, short targetElo, int eloDifference, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<RankedGame>> WaitForGameConfirmationAsync(string joinedGameId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result> InsertGameAsync(RankedGame game, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<RankedGame>> GetClosestGameSearchAsync(int searchedElo, CancellationToken cancellationToken, int timeoutSeconds = 120);
}