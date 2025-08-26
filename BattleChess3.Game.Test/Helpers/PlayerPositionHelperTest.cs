using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Helpers;
using BattleChess3.Game.Players;
using Xunit;
using Assert = Xunit.Assert;

namespace BattleChess3.Game.Test.Helpers;

public class PlayerPositionHelperTest
{
    [Fact]
    public void WhenBlack_DoesNotChange()
    {
        var position = new Position(3, 3);

        var relative = PlayerPositionHelper.GetRelativePosition(Player.Black, position);

        Assert.Equal(relative, new Position(3, 3));
    }

    [Fact]
    public void WhenWhite_InvertsY()
    {
        var position = new Position(3, 3);

        var relative = PlayerPositionHelper.GetRelativePosition(Player.White, position);

        Assert.Equal(relative, new Position(3, 4));
    }

    [Fact]
    public void WhenNonPlayer_ThrowsException()
    {
        var position = new Position(3, 3);

        Action action = () => PlayerPositionHelper.GetRelativePosition(Player.Neutral, position);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}