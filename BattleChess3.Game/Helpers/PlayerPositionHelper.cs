using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.Helpers;

public static class PlayerPositionHelper
{
    public static Position GetRelativePosition(Player player, Position absPosition)
    {
        return player switch
        {
            Player.White => new Position(absPosition.X, Constants.BoardLength - absPosition.Y - 1),
            Player.Black => new Position(absPosition.X, absPosition.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(player))
        };
    }
}