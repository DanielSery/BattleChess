using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.Core.Players;
using BattleChess3.Game.Players;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace BattleChess3.Game.Test.Figures;

public class FigureTests
{
    [Fact]
    public void NoneFigureType_FigureIsValid()
    {
        var player = new ControlledPlayerInfo(Player.Neutral, string.Empty);
        _ = new Figure(player, NoneFigureType.Instance, false);
    }

    [Fact]
    public void NeutralPlayer_ThrowsWhenNotHavingNeutralImage()
    {
        var player = new ControlledPlayerInfo(Player.Neutral, string.Empty);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {1, new Uri("component/Images/test.png", UriKind.Relative)},
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        Assert.Throws<ArgumentException>(() => _ = new Figure(player, figureTypeMock.Object, false));
    }

    [Fact]
    public void NeutralPlayer_NotThrowsWhenHavingNeutralImage()
    {
        var player = new ControlledPlayerInfo(Player.Neutral, string.Empty);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {0, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        _ = new Figure(player, figureTypeMock.Object, false);
    }

    [Fact]
    public void WhitePlayer_ThrowsWhenNotHavingWhiteImage()
    {
        var player = new ControlledPlayerInfo(Player.White, string.Empty);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {0, new Uri("component/Images/test.png", UriKind.Relative)},
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        Assert.Throws<ArgumentException>(() => _ = new Figure(player, figureTypeMock.Object, false));
    }

    [Fact]
    public void WhitePlayer_NotThrowsWhenHavingWhiteImage()
    {
        var player = new ControlledPlayerInfo(Player.White, string.Empty);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {1, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        _ = new Figure(player, figureTypeMock.Object, false);
    }

    [Fact]
    public void BlackPlayer_ThrowsWhenNotHavingBlackImage()
    {
        var player = new ControlledPlayerInfo(Player.Black, string.Empty);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {0, new Uri("component/Images/test.png", UriKind.Relative)},
            {1, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        Assert.Throws<ArgumentException>(() => _ = new Figure(player, figureTypeMock.Object, false));
    }

    [Fact]
    public void BlackPlayer_NotThrowsWhenHavingBlackImage()
    {
        var player = new ControlledPlayerInfo(Player.Black, string.Empty);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        _ = new Figure(player, figureTypeMock.Object, false);
    }

    [Fact]
    public void GetsFieldsFromFigureType()
    {
        var player = new ControlledPlayerInfo(Player.Black, string.Empty);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        figureTypeMock.Setup(x => x.DisplayName).Returns("Test");

        var figure = new Figure(player, figureTypeMock.Object, false);

        Assert.Equal("Test", figure.DisplayName);
    }

    [Fact]
    public void GetsUriFromFigureType()
    {
        var player = new ControlledPlayerInfo(Player.Black, string.Empty);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        figureTypeMock.Setup(x => x.DisplayName).Returns("Test");

        var figure = new Figure(player, figureTypeMock.Object, false);

        Assert.Equal(new Uri("component/Images/test.png", UriKind.Relative), figure.ImageUri);
    }

    [Fact]
    public void GetsPossibleActionsFromFigureType()
    {
        var player = new ControlledPlayerInfo(Player.Black, string.Empty);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        figureTypeMock.Setup(x => x.DisplayName).Returns("Test");

        var figure = new Figure(player, figureTypeMock.Object, false);
        figure.GetPossibleActions(Mock.Of<ITile>(), Mock.Of<IBoard>());

        figureTypeMock.Verify(x => x.GetPossibleActions(It.IsAny<ITile>(), It.IsAny<IBoard>()), Times.Once);
    }
}