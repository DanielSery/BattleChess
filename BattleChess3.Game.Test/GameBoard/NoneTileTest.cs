using BattleChess3.Core.Figures;
using BattleChess3.Core.Players;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;
using Xunit;
using Assert = Xunit.Assert;

namespace BattleChess3.Game.Test.GameBoard;

public class NoneTileTest
{
    [Fact]
    public void NoneTileHasNoneFigure()
    {
        Assert.Equal(Figure.None.Id, NoneTile.Instance.Figure.Id);
    }

    [Fact]
    public void AfterSettingNoneTileHasNoneFigure()
    {
        var noneTile = NoneTile.Instance;
        noneTile.Figure = new Figure(new ControlledPlayerInfo(Player.Neutral, ""), NoneFigureType.Instance, false);
        Assert.Equal(Figure.None.Id, noneTile.Figure.Id);
    }

    [Fact]
    public void NonTilePositionIndexIsOutsideOfBoard()
    {
        Assert.NotInRange(NoneTile.Instance.RelativePosition.GetIndex(), 0, 63);
    }
}