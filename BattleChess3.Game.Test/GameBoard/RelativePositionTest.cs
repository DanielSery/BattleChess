using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;
using Moq;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace BattleChess3.Game.Test.GameBoard;

public class RelativePositionTest
{
    private readonly ITile _tile;
    private readonly Figure _figure;

    public RelativePositionTest()
    {
        _figure = new Figure(PlayerInfo.Neutral, NoneFigureType.Instance, false);

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

        Assert.AreEqual(1, relative.RelativePosition.X, "rel x");
        Assert.AreEqual(6, relative.RelativePosition.Y, "rel y");
        Assert.AreEqual(1, relative.AbsolutePosition.X, "abs X");
        Assert.AreEqual(1, relative.AbsolutePosition.Y, "abs Y");
        Assert.AreEqual(_figure, relative.Figure, "figure");
    }

    [Fact]
    public void ForBlackPlayer_RelativeNormalY()
    {
        var relative = new RelativeTile(_tile, Player.Black);

        Assert.AreEqual(1, relative.RelativePosition.X, "rel x");
        Assert.AreEqual(1, relative.RelativePosition.Y, "rel y");
        Assert.AreEqual(1, relative.AbsolutePosition.X, "abs X");
        Assert.AreEqual(1, relative.AbsolutePosition.Y, "abs Y");
        Assert.AreEqual(_figure, relative.Figure, "figure");
    }

    [Fact]
    public void ForDoubleRelative_LastWhite_RelativeInvertedY()
    {
        ITile relative = new RelativeTile(_tile, Player.White);
        relative = relative.GetRelativeTile(Player.White);

        Assert.AreEqual(1, relative.RelativePosition.X, "rel x");
        Assert.AreEqual(6, relative.RelativePosition.Y, "rel y");
        Assert.AreEqual(1, relative.AbsolutePosition.X, "abs X");
        Assert.AreEqual(1, relative.AbsolutePosition.Y, "abs Y");
        Assert.AreEqual(_figure, relative.Figure, "figure");
    }

    [Fact]
    public void ForDoubleRelative_LastBlack_RelativeNormalY()
    {
        ITile relative = new RelativeTile(_tile, Player.White);
        relative = relative.GetRelativeTile(Player.Black);

        Assert.AreEqual(1, relative.RelativePosition.X, "rel x");
        Assert.AreEqual(1, relative.RelativePosition.Y, "rel y");
        Assert.AreEqual(1, relative.AbsolutePosition.X, "abs X");
        Assert.AreEqual(1, relative.AbsolutePosition.Y, "abs Y");
        Assert.AreEqual(_figure, relative.Figure, "figure");
    }
}