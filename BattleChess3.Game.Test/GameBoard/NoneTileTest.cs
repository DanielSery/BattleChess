using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;
using Moq;
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
        noneTile.Figure = new Figure(new PlayerInfo("", "", null, 0), Mock.Of<IFigureType>(), false);
        Assert.Equal(Figure.None.Id, noneTile.Figure.Id);
    }

    [Fact]
    public void NonTilePositionIndexIsOutsideOfBoard()
    {
        Assert.NotInRange(NoneTile.Instance.Position.GetIndex(), 0, 63);
    }
}