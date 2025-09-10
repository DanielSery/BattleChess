using AwesomeAssertions;
using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Players;
using CrownsGuard.Maps.Figures;
using Moq;

namespace CrownsGuard.Maps.Test.Figures;

public class FigureCreatorTest
{
    [Fact]
    public void CreateFigure_UsesInformationFromBlueprint()
    {
        var figureOwnerMock = new Mock<IPlayer>();
        figureOwnerMock.Setup(x => x.PlayerColor).Returns(PlayerColor.Black);
        var figureOwnersMock = new Mock<IPlayersOwner>();
        figureOwnersMock.Setup(x => x.GetPlayer(PlayerColor.Black)).Returns(figureOwnerMock.Object);

        var figureGroupMock = new Mock<IFigureTypeInfoGroup>();
        var figureTypeMock = new Mock<IFigureTypeInfo>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {1, new Uri("component/Images/test.png", UriKind.Relative)},
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        figureGroupMock.Setup(x => x.GetFigureTypeById(23)).Returns(figureTypeMock.Object);

        var figureCreator = new FigureCreator(figureOwnersMock.Object, figureGroupMock.Object);

        var figure = figureCreator.CreateFigure(new FigureBlueprint(PlayerColor.Black, 23, true));

        figure.Owner.Should().Be(figureOwnerMock.Object);
        figure.TypeInfo.Should().Be(figureTypeMock.Object);
        figure.IsKing.Should().BeTrue();
    }

    [Fact]
    public void CreateEmptyFigure_CreatesEmptyFigure()
    {
        var figureOwnersMock = new Mock<IPlayersOwner>();

        var figureGroupMock = new Mock<IFigureTypeInfoGroup>();
        var figureTypeMock = new Mock<IFigureTypeInfo>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {0, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        figureGroupMock.Setup(x => x.GetFigureTypeById(0)).Returns(figureTypeMock.Object);

        var figureCreator = new FigureCreator(figureOwnersMock.Object, figureGroupMock.Object);

        var figure = figureCreator.CreateEmptyFigure();

        figure.Owner.Should().Be(NeutralPlayer.Instance);
        figure.TypeInfo.Should().Be(figureTypeMock.Object);
        figure.IsKing.Should().BeFalse();
    }
}