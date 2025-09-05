using AwesomeAssertions;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.Test.Helpers;

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