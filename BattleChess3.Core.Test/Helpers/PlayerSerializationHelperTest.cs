using BattleChess3.Core.Helpers;
using BattleChess3.Core.Players;

namespace BattleChess3.Core.Test.Helpers;

public class PlayerSerializationHelperTest
{
    [Fact]
    public void ToInt_ThrowsError_WhenInvalidPlayer()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ((Player)5).ToInt());
    }

    [Fact]
    public void ToPlayer_ThrowsError_WhenInvalidPosition()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PlayerSerializationHelper.ToPlayer(5));
    }

    [Fact]
    public void ConversionBack_ReturnsInitialObject()
    {
        var player = Player.Black;
        var playerInt = player.ToInt();
        var doubleConverted = PlayerSerializationHelper.ToPlayer(playerInt);

        Assert.Equal(player, doubleConverted);
    }
}