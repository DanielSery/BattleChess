using CrownsGuard.Core;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;

namespace CrownsGuard.Game.Helpers;

public static class RelativePositionHelper
{
    public static Position GetRelative(PlayerColor playerColor, Position absPosition)
    {
        return playerColor switch
        {
            PlayerColor.White => new Position(absPosition.X, (sbyte)(Constants.BoardLength - absPosition.Y - 1)),
            PlayerColor.Black => absPosition,
            _ => throw new ArgumentOutOfRangeException(nameof(playerColor))
        };
    }
}