using FluentResults;

namespace CrownsGuard.Database.Players;

public interface IPlayersCollectionHandler
{
    Task<Result<RegisteredPlayer>> FindByIdAsync(string id, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result<RegisteredPlayer>> FindByNameAsync(string name, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result<RegisteredPlayer>> FindByEmailHashAsync(string emailHash, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result> InsertAsync(RegisteredPlayer player, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result> UpdateEloAsync(string playerId, int newElo, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result> UpdateSetupAsync(string playerId, int[] newMap, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result> UpdateUnlockedFiguresAsync(string playerId, byte[] newUnlockedFigures, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<RegisteredPlayer>> WaitForEloUpdateAsync(string playerId, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result<List<PublicPlayerData>>> GetTopLeaderboardAsync(CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result<List<PublicPlayerData>>> GetUserLeaderboardAsync(string playerId, CancellationToken cancellationToken, int timeoutSeconds = 120);
}