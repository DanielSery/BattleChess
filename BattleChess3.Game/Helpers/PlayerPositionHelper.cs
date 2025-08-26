using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.Helpers;

public static class PlayerPositionHelper
{
    public static Position GetPlayerPOVPosition(in int playerId, Position pos)
    {
        return playerId switch
        {
            1 => new Position(pos.X, Constants.BoardLength - pos.Y - 1),
            2 => new Position(pos.X, pos.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(playerId))
        };
    }

    public static Position GetPlayerPOVPosition(in Player currentPlayer, Position pos)
        => GetPlayerPOVPosition(currentPlayer.Index, pos);
}