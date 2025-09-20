using CrownsGuard.Core.Figures;
using CrownsGuard.Database.Ranked;
using FluentResults;

namespace CrownsGuard.Multiplayer.Ranked;

public interface IMultiplayerRankedService
{
    Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoin gameSearchJoin)>> FindRankedGameAsync(Figure[] myMap, CancellationToken cancellationToken);
}