using CrownsGuard.Core.GameBoard;
using CrownsGuard.Database.Ranked;
using FluentResults;

namespace CrownsGuard.Multiplayer.Ranked;

public interface IMultiplayerRankedService
{
    Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoin gameSearchJoin)>> FindRankedGameAsync(BoardBlueprint myMap, CancellationToken cancellationToken);
}