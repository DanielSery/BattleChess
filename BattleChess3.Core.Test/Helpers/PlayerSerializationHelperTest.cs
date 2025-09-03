using AwesomeAssertions;
using BattleChess3.Core.Helpers;
using BattleChess3.Core.Players;

namespace BattleChess3.Core.Test.Helpers;

public class PlayerSerializationHelperTest
{
    [Fact]
    public void ToInt_ThrowsError_WhenInvalidPlayer()
    {
        Action toIntAction = () => ((Player)5).ToInt();

        toIntAction.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToPlayer_ThrowsError_WhenInvalidPosition()
    {
        Action toPlayerAction = () => PlayerSerializationHelper.ToPlayer(5);

        toPlayerAction.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ConversionBack_ReturnsInitialObject()
    {
        var player = Player.Black;
        var playerInt = player.ToInt();
        var doubleConverted = PlayerSerializationHelper.ToPlayer(playerInt);

        doubleConverted.Should().Be(player);
    }
}