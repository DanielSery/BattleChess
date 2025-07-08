using BattleChess3.Maps;
using BattleChess3.Multiplayer.Tables;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerRankedService
{
    Task<Result<(bool isHost, GameSearch gameSearch, GameSearchJoin gameSearchJoin)>> FindRankedGame(MapBlueprint myMap);
}