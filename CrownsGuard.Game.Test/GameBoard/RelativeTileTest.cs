using AwesomeAssertions;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Game.GameBoard;
using Moq;

namespace CrownsGuard.Game.Test.GameBoard;

public class RelativeTileTest
{
    private readonly ITile _tile;
    private readonly FigureInfo _figure;

    public RelativeTileTest()
    {
        _figure = new FigureInfo(NeutralFigureOwner.Instance, NoneFigureType.Instance, false);

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

        relative.RelativePosition.X.Should().Be(1);
        relative.RelativePosition.Y.Should().Be(6);
        relative.AbsolutePosition.X.Should().Be(1);
        relative.AbsolutePosition.Y.Should().Be(1);
        relative.Figure.Should().Be(_figure);
    }

    [Fact]
    public void ForBlackPlayer_RelativeNormalY()
    {
        var relative = new RelativeTile(_tile, Player.Black);

        relative.RelativePosition.X.Should().Be(1);
        relative.RelativePosition.Y.Should().Be(1);
        relative.AbsolutePosition.X.Should().Be(1);
        relative.AbsolutePosition.Y.Should().Be(1);
        relative.Figure.Should().Be(_figure);
    }

    [Fact]
    public void ForDoubleRelative_LastWhite_RelativeInvertedY()
    {
        ITile relative = new RelativeTile(_tile, Player.White);
        relative = relative.GetRelativeTile(Player.White);

        relative.RelativePosition.X.Should().Be(1);
        relative.RelativePosition.Y.Should().Be(6);
        relative.AbsolutePosition.X.Should().Be(1);
        relative.AbsolutePosition.Y.Should().Be(1);
        relative.Figure.Should().Be(_figure);
    }

    [Fact]
    public void ForDoubleRelative_LastBlack_RelativeNormalY()
    {
        ITile relative = new RelativeTile(_tile, Player.White);
        relative = relative.GetRelativeTile(Player.Black);

        relative.RelativePosition.X.Should().Be(1);
        relative.RelativePosition.Y.Should().Be(1);
        relative.AbsolutePosition.X.Should().Be(1);
        relative.AbsolutePosition.Y.Should().Be(1);
        relative.Figure.Should().Be(_figure);
    }
}