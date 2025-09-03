using AwesomeAssertions;
using BattleChess3.Core.GameBoard;
using BattleChess3.Core.Players;
using BattleChess3.Game.Helpers;

namespace BattleChess3.Game.Test.Helpers;

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