using FluentResults;

namespace CrownsGuard.Database.Players;

public interface IPlayersCollectionHandler
{
    Task<Result<RegisteredPlayer>> FindPlayerByIdAsync(string id, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result<RegisteredPlayer>> FindPlayerByNameAsync(string name, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result<RegisteredPlayer>> FindPlayerByEmailHashAsync(string emailHash, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result> InsertPlayerAsync(RegisteredPlayer player, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result> UpdatePlayerEloAsync(string playerId, int newElo, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result> UpdatePlayerSetupAsync(string playerId, int[] newMap, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result> UpdatePlayerUnlockedFiguresAsync(string playerId, byte[] newUnlockedFigures, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<RegisteredPlayer>> WaitForPlayerEloUpdateAsync(string playerId, int initialElo, CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result<List<PublicPlayerData>>> GetTopLeaderboardAsync(CancellationToken cancellationToken, int timeoutSeconds = 120);
    Task<Result<List<PublicPlayerData>>> GetUserLeaderboardAsync(string playerId, CancellationToken cancellationToken, int timeoutSeconds = 120);
}