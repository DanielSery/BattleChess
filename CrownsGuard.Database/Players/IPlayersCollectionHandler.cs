using FluentResults;

namespace CrownsGuard.Database.Players;

public interface IPlayersCollectionHandler
{
    Task<Result<RegisteredPlayer>> FindByIdAsync(string id, CancellationToken cancellationToken);

    Task<Result<RegisteredPlayer>> FindByNameAsync(string name, CancellationToken cancellationToken);

    Task<Result<RegisteredPlayer>> FindByEmailHashAsync(string emailHash, CancellationToken cancellationToken);

    Task<Result> InsertAsync(RegisteredPlayer player, CancellationToken cancellationToken);
    Task<Result> UpdateEloAsync(string playerId, int newElo, CancellationToken cancellationToken);
    Task<Result> UpdateSetupAsync(string playerId, int[] newMap, CancellationToken cancellationToken);
    Task<Result> UpdateUnlockedFiguresAsync(string playerId, byte[] newUnlockedFigures, CancellationToken cancellationToken);

    Task<Result<RegisteredPlayer>> WaitForEloUpdateAsync(string playerId, CancellationToken cancellationToken);
    Task<Result<List<PublicPlayerData>>> GetTopLeaderboardAsync(CancellationToken cancellationToken);
    Task<Result<List<PublicPlayerData>>> GetUserLeaderboardAsync(string playerId, CancellationToken cancellationToken);
}