using BattleChess3.Game.GameBoard;
using BattleChess3.Multiplayer.Tables;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerRankedService
{
    Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoin gameSearchJoin)>> FindRankedGameAsync(BoardBlueprint myMap, CancellationToken cancellationToken);
}