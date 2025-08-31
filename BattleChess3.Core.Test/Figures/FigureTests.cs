using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.Core.Players;
using Moq;
using Assert = Xunit.Assert;

namespace BattleChess3.Core.Test.Figures;

public class FigureTests
{
    [Fact]
    public void NoneFigureType_FigureIsValid()
    {
        var player = GetFigureOwner(Player.Neutral);
        _ = new Figure(player, NoneFigureType.Instance, false);
    }

    [Fact]
    public void NeutralPlayer_ThrowsWhenNotHavingNeutralImage()
    {
        var player = GetFigureOwner(Player.Neutral);
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
        var player = GetFigureOwner(Player.Neutral);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {0, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        _ = new Figure(player, figureTypeMock.Object, false);
    }

    [Fact]
    public void WhitePlayer_ThrowsWhenNotHavingWhiteImage()
    {
        var player = GetFigureOwner(Player.White);
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
        var player = GetFigureOwner(Player.White);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {1, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        _ = new Figure(player, figureTypeMock.Object, false);
    }

    [Fact]
    public void BlackPlayer_ThrowsWhenNotHavingBlackImage()
    {
        var player = GetFigureOwner(Player.Black);
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
        var player = GetFigureOwner(Player.Black);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        _ = new Figure(player, figureTypeMock.Object, false);
    }

    [Fact]
    public void GetsFieldsFromFigureType()
    {
        var player = GetFigureOwner(Player.Black);
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
        var player = GetFigureOwner(Player.Black);
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
        var player = GetFigureOwner(Player.Black);
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        figureTypeMock.Setup(x => x.DisplayName).Returns("Test");

        var figure = new Figure(player, figureTypeMock.Object, false);
        figure.GetPossibleActions(Mock.Of<ITile>(), Mock.Of<IBoard>());

        figureTypeMock.Verify(x => x.GetPossibleActions(It.IsAny<ITile>(), It.IsAny<IBoard>()), Times.Once);
    }

    private IFigureOwner GetFigureOwner(Player player)
    {
        var mock = new Mock<IFigureOwner>();
        mock.Setup(x => x.Player).Returns(player);
        return mock.Object;
    }
}