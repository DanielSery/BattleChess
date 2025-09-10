using AwesomeAssertions;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Players;
using CrownsGuard.Game.GameBoard;
using CrownsGuard.Game.Players;

namespace CrownsGuard.Game.Test.GameBoard;

public class NoneTileTest
{
    [Fact]
    public void NoneTileHasNoneFigure()
    {
        var noneFigureId = NoneTile.Instance.Figure.Id;

        noneFigureId.Should().Be(FigureWithInfo.None.Id);
    }

    [Fact]
    public void AfterSettingNoneTileHasNoneFigure()
    {
        var noneTile = NoneTile.Instance;
        noneTile.Figure = new FigureWithInfo(new ControlledPlayerInfo(PlayerColor.Neutral, ""), NoneFigureTypeInfo.Instance, false);
        var noneFigureId = noneTile.Figure.Id;

        noneFigureId.Should().Be(FigureWithInfo.None.Id);
    }

    [Fact]
    public void NonTilePositionIndexIsOutsideOfBoard()
    {
        var relativeIndex = NoneTile.Instance.RelativePosition.GetIndex();

        relativeIndex.Should().NotBeInRange(0, 63);
    }
}