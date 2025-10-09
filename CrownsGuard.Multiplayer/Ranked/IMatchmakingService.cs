using CrownsGuard.Core.Figures;
using CrownsGuard.Database.Ranked;
using FluentResults;

namespace CrownsGuard.Multiplayer.Ranked;

public interface IMatchmakingService
{
    Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoin gameSearchJoin)>> FindRankedGameAsync(
        string currentPlayerId, short currentPlayerElo, Figure[] currentPlayerMap, CancellationToken cancellationToken);
}