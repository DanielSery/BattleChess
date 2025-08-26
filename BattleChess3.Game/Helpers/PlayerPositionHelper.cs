using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.Helpers;

public static class PlayerPositionHelper
{
    public static Position GetPlayerRelativePosition(in Player player, Position pos)
    {
        return player switch
        {
            Player.White => new Position(pos.X, Constants.BoardLength - pos.Y - 1),
            Player.Black => new Position(pos.X, pos.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(player))
        };
    }

    public static Position GetPlayerRelativePosition(in PlayerInfo currentPlayerInfo, Position pos)
        => GetPlayerRelativePosition(currentPlayerInfo.Player, pos);
}