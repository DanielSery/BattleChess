using AwesomeAssertions;
using BattleChess3.Core.Figures;
using BattleChess3.Core.Players;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.Test.GameBoard;

public class NoneTileTest
{
    [Fact]
    public void NoneTileHasNoneFigure()
    {
        var noneFigureId = NoneTile.Instance.Figure.Id;

        noneFigureId.Should().Be(Figure.None.Id);
    }

    [Fact]
    public void AfterSettingNoneTileHasNoneFigure()
    {
        var noneTile = NoneTile.Instance;
        noneTile.Figure = new Figure(new ControlledPlayerInfo(Player.Neutral, ""), NoneFigureType.Instance, false);
        var noneFigureId = noneTile.Figure.Id;

        noneFigureId.Should().Be(Figure.None.Id);
    }

    [Fact]
    public void NonTilePositionIndexIsOutsideOfBoard()
    {
        var relativeIndex = NoneTile.Instance.RelativePosition.GetIndex();

        relativeIndex.Should().NotBeInRange(0, 63);
    }
}