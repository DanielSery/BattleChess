using BattleChess3.Maps;
using BattleChess3.Multiplayer.Tables;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerRankedService
{
    Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoins gameSearchJoin)>> FindRankedGameAsync(MapBlueprint myMap, CancellationToken cancellationToken);
}