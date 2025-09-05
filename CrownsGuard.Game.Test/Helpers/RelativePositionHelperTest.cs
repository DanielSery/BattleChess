using AwesomeAssertions;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Game.Helpers;

namespace CrownsGuard.Game.Test.Helpers;

public class RelativePositionHelperTest
{
    [Fact]
    public void WhenBlack_DoesNotChange()
    {
        var position = new Position(3, 3);

        var relative = RelativePositionHelper.GetRelative(Player.Black, position);

        new Position(3, 3).Should().Be(relative);
    }

    [Fact]
    public void WhenWhite_InvertsY()
    {
        var position = new Position(3, 3);

        var relative = RelativePositionHelper.GetRelative(Player.White, position);

        new Position(3, 4).Should().Be(relative);
    }

    [Fact]
    public void WhenNonPlayer_ThrowsException()
    {
        var position = new Position(3, 3);

        Action action = () => RelativePositionHelper.GetRelative(Player.Neutral, position);

        action.Should().Throw<ArgumentOutOfRangeException>();

    }
}