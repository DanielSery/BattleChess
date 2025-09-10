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
    private readonly FigureWithInfo _figure;

    public RelativeTileTest()
    {
        _figure = new FigureWithInfo(NeutralPlayer.Instance, NoneFigureTypeInfo.Instance, false);

        var tileMock = new Mock<ITile>();
        tileMock.Setup(x => x.RelativePosition).Returns(new Position(1, 1));
        tileMock.Setup(x => x.Position).Returns(new Position(1, 1));
        tileMock.Setup(x => x.Figure).Returns(_figure);
        _tile = tileMock.Object;
    }

    [Fact]
    public void ForWhitePlayer_RelativeInvertedY()
    {
        var relative = new RelativeTile(_tile, PlayerColor.White);

        relative.RelativePosition.X.Should().Be(1);
        relative.RelativePosition.Y.Should().Be(6);
        relative.AbsolutePosition.X.Should().Be(1);
        relative.AbsolutePosition.Y.Should().Be(1);
        relative.Figure.Should().Be(_figure);
    }

    [Fact]
    public void ForBlackPlayer_RelativeNormalY()
    {
        var relative = new RelativeTile(_tile, PlayerColor.Black);

        relative.RelativePosition.X.Should().Be(1);
        relative.RelativePosition.Y.Should().Be(1);
        relative.AbsolutePosition.X.Should().Be(1);
        relative.AbsolutePosition.Y.Should().Be(1);
        relative.Figure.Should().Be(_figure);
    }

    [Fact]
    public void ForDoubleRelative_LastWhite_RelativeInvertedY()
    {
        ITile relative = new RelativeTile(_tile, PlayerColor.White);
        relative = relative.GetRelativeTile(PlayerColor.White);

        relative.RelativePosition.X.Should().Be(1);
        relative.RelativePosition.Y.Should().Be(6);
        relative.Position.X.Should().Be(1);
        relative.Position.Y.Should().Be(1);
        relative.Figure.Should().Be(_figure);
    }

    [Fact]
    public void ForDoubleRelative_LastBlack_RelativeNormalY()
    {
        ITile relative = new RelativeTile(_tile, PlayerColor.White);
        relative = relative.GetRelativeTile(PlayerColor.Black);

        relative.RelativePosition.X.Should().Be(1);
        relative.RelativePosition.Y.Should().Be(1);
        relative.Position.X.Should().Be(1);
        relative.Position.Y.Should().Be(1);
        relative.Figure.Should().Be(_figure);
    }
}