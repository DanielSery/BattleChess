using BattleChess3.Core.Players;

namespace BattleChess3.Core.Helpers;

public static class PlayerSerializationHelper
{
    public static int ToInt(this Player player)
    {
        var value = (int)player;
        if (value is < (int)Player.Neutral or > (int)Player.Black)
            throw new ArgumentOutOfRangeException(nameof(player), player, $"Player {value} is out of range.");

        return value;
    }

    public static Player ToPlayer(int player)
    {
        if (player is < (int)Player.Neutral or > (int)Player.Black)
            throw new ArgumentOutOfRangeException(nameof(player), player, $"Player {player} is out of range.");

        return (Player)player;
    }
}