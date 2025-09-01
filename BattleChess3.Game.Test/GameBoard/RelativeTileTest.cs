using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.Core.Players;
using BattleChess3.Game.GameBoard;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace BattleChess3.Game.Test.GameBoard;

public class RelativeTileTest
{
    private readonly ITile _tile;
    private readonly Figure _figure;

    public RelativeTileTest()
    {
        _figure = new Figure(NeutralFigureOwner.Instance, NoneFigureType.Instance, false);

        var tileMock = new Mock<ITile>();
        tileMock.Setup(x => x.RelativePosition).Returns(new Position(1, 1));
        tileMock.Setup(x => x.AbsolutePosition).Returns(new Position(1, 1));
        tileMock.Setup(x => x.Figure).Returns(_figure);
        _tile = tileMock.Object;
    }

    [Fact]
    public void ForWhitePlayer_RelativeInvertedY()
    {
        var relative = new RelativeTile(_tile, Player.White);

        Assert.Equal(1, relative.RelativePosition.X);
        Assert.Equal(6, relative.RelativePosition.Y);
        Assert.Equal(1, relative.AbsolutePosition.X);
        Assert.Equal(1, relative.AbsolutePosition.Y);
        Assert.Equal(_figure, relative.Figure);
    }

    [Fact]
    public void ForBlackPlayer_RelativeNormalY()
    {
        var relative = new RelativeTile(_tile, Player.Black);

        Assert.Equal(1, relative.RelativePosition.X);
        Assert.Equal(1, relative.RelativePosition.Y);
        Assert.Equal(1, relative.AbsolutePosition.X);
        Assert.Equal(1, relative.AbsolutePosition.Y);
        Assert.Equal(_figure, relative.Figure);
    }

    [Fact]
    public void ForDoubleRelative_LastWhite_RelativeInvertedY()
    {
        ITile relative = new RelativeTile(_tile, Player.White);
        relative = relative.GetRelativeTile(Player.White);

        Assert.Equal(1, relative.RelativePosition.X);
        Assert.Equal(6, relative.RelativePosition.Y);
        Assert.Equal(1, relative.AbsolutePosition.X);
        Assert.Equal(1, relative.AbsolutePosition.Y);
        Assert.Equal(_figure, relative.Figure);
    }

    [Fact]
    public void ForDoubleRelative_LastBlack_RelativeNormalY()
    {
        ITile relative = new RelativeTile(_tile, Player.White);
        relative = relative.GetRelativeTile(Player.Black);

        Assert.Equal(1, relative.RelativePosition.X);
        Assert.Equal(1, relative.RelativePosition.Y);
        Assert.Equal(1, relative.AbsolutePosition.X);
        Assert.Equal(1, relative.AbsolutePosition.Y);
        Assert.Equal(_figure, relative.Figure);
    }
}