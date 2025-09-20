using CrownsGuard.Game.Players;

namespace CrownsGuard.Game.Helpers;

public static class PlayerSerializationHelper
{
    public static int ToInt(this PlayerColor playerColor)
    {
        return playerColor switch
        {
            PlayerColor.Black => 2,
            PlayerColor.Neutral => 0,
            PlayerColor.White => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(playerColor), playerColor, null)
        };
    }

    public static PlayerColor ToPlayer(int player)
    {
        return player switch
        {
            0 => PlayerColor.Neutral,
            1 => PlayerColor.White,
            2 => PlayerColor.Black,
            _ => throw new ArgumentOutOfRangeException(nameof(player), player, null)
        };
    }
}