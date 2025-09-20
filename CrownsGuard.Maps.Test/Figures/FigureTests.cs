using AwesomeAssertions;
using CrownsGuard.FigureDefinitions.Figures;
using CrownsGuard.Game.Players;
using CrownsGuard.Maps.Figures;
using Moq;

namespace CrownsGuard.Core.Test.Figures;

public class FigureTests
{
    [Fact]
    public void NoneFigureType_FigureIsValid()
    {
        var player = GetFigureOwner(PlayerColor.Neutral);
        _ = new FigureWithInfo(player, NoneFigureTypeInfo.Instance, false);
    }

    [Fact]
    public void NeutralPlayer_ThrowsWhenNotHavingNeutralImage()
    {
        var player = GetFigureOwner(PlayerColor.Neutral);
        var figureTypeMock = new Mock<IFigureTypeInfo>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {1, new Uri("component/Images/test.png", UriKind.Relative)},
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });

        Action createFigureAction = () => _ = new FigureWithInfo(player, figureTypeMock.Object, false);

        createFigureAction.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NeutralPlayer_NotThrowsWhenHavingNeutralImage()
    {
        var player = GetFigureOwner(PlayerColor.Neutral);
        var figureTypeMock = new Mock<IFigureTypeInfo>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {0, new Uri("component/Images/test.png", UriKind.Relative)},
        });

        Action createFigureAction = () => _ = new FigureWithInfo(player, figureTypeMock.Object, false);

        createFigureAction.Should().NotThrow();
    }

    [Fact]
    public void WhitePlayer_ThrowsWhenNotHavingWhiteImage()
    {
        var player = GetFigureOwner(PlayerColor.White);
        var figureTypeMock = new Mock<IFigureTypeInfo>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {0, new Uri("component/Images/test.png", UriKind.Relative)},
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });

        Action createFigureAction = () => _ = new FigureWithInfo(player, figureTypeMock.Object, false);

        createFigureAction.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WhitePlayer_NotThrowsWhenHavingWhiteImage()
    {
        var player = GetFigureOwner(PlayerColor.White);
        var figureTypeMock = new Mock<IFigureTypeInfo>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {1, new Uri("component/Images/test.png", UriKind.Relative)},
        });

        Action createFigureAction = () => _ = new FigureWithInfo(player, figureTypeMock.Object, false);

        createFigureAction.Should().NotThrow();
    }

    [Fact]
    public void BlackPlayer_ThrowsWhenNotHavingBlackImage()
    {
        var player = GetFigureOwner(PlayerColor.Black);
        var figureTypeMock = new Mock<IFigureTypeInfo>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {0, new Uri("component/Images/test.png", UriKind.Relative)},
            {1, new Uri("component/Images/test.png", UriKind.Relative)},
        });

        Action createFigureAction = () => _ = new FigureWithInfo(player, figureTypeMock.Object, false);

        createFigureAction.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void BlackPlayer_NotThrowsWhenHavingBlackImage()
    {
        var player = GetFigureOwner(PlayerColor.Black);
        var figureTypeMock = new Mock<IFigureTypeInfo>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });

        Action createFigureAction = () => _ = new FigureWithInfo(player, figureTypeMock.Object, false);

        createFigureAction.Should().NotThrow();
    }

    [Fact]
    public void GetsFieldsFromFigureType()
    {
        var player = GetFigureOwner(PlayerColor.Black);
        var figureTypeMock = new Mock<IFigureTypeInfo>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        figureTypeMock.Setup(x => x.DisplayName).Returns("Test");

        var figure = new FigureWithInfo(player, figureTypeMock.Object, false);

        figure.DisplayName.Should().Be("Test");
    }

    [Fact]
    public void GetsUriFromFigureType()
    {
        var player = GetFigureOwner(PlayerColor.Black);
        var figureTypeMock = new Mock<IFigureTypeInfo>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        figureTypeMock.Setup(x => x.DisplayName).Returns("Test");

        var figure = new FigureWithInfo(player, figureTypeMock.Object, false);

        figure.ImageUri.Should().Be(new Uri("component/Images/test.png", UriKind.Relative));
    }

    private static IPlayer GetFigureOwner(PlayerColor playerColor)
    {
        var mock = new Mock<IPlayer>();
        mock.Setup(x => x.PlayerColor).Returns(playerColor);
        return mock.Object;
    }
}