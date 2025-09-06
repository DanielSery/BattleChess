using FluentResults;

namespace CrownsGuard.Database.Ranked;

public interface IRankedGamesCollectionHandler
{
    Task<Result> TryConfirmGameJoin(string gameId, string joinId, CancellationToken cancellationToken);

    Task<Result> DeleteGameSearch(string deletedGameId, CancellationToken cancellationToken);

    Task<Result<RankedGame>> FindRankedGame(string gameId, short targetElo, int eloDifference, CancellationToken cancellationToken);

    Task<Result<RankedGame>> WaitForGameAccept(string joinedGameId, int timeoutSeconds, CancellationToken cancellationToken);

    Task<Result> InsertRankedGame(RankedGame game, CancellationToken cancellationToken);

    Task<Result<RankedGame>> GetClosestGameSearchAsync(int searchedElo, int maxDifference, CancellationToken cancellationToken);
}