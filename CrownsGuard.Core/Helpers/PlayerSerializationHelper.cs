using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.Helpers;

public static class PlayerSerializationHelper
{
    public static int ToInt(this PlayerColor playerColor)
    {
        var value = (int)playerColor;
        if (value is < (int)PlayerColor.Neutral or > (int)PlayerColor.Black)
            throw new ArgumentOutOfRangeException(nameof(playerColor), playerColor, $"Player {value} is out of range.");

        return value;
    }

    public static PlayerColor ToPlayer(int player)
    {
        if (player is < (int)PlayerColor.Neutral or > (int)PlayerColor.Black)
            throw new ArgumentOutOfRangeException(nameof(player), player, $"Player {player} is out of range.");

        return (PlayerColor)player;
    }
}