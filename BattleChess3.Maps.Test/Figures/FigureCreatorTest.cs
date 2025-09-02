using BattleChess3.Core;
using BattleChess3.Core.Figures;
using BattleChess3.Core.Players;
using BattleChess3.Maps.Figures;
using Moq;

namespace BattleChess3.Maps.Test.Figures;

public class FigureCreatorTest
{
    [Fact]
    public void CreateFigure_UsesInformationFromBlueprint()
    {
        var figureOwnerMock = new Mock<IFigureOwner>();
        figureOwnerMock.Setup(x => x.Player).Returns(Player.Black);
        figureOwnerMock.Setup(x => x.Figures).Returns([]);
        var figureOwnersMock = new Mock<IFigureOwnersHolder>();
        figureOwnersMock.Setup(x => x.GetFigureOwner(Player.Black)).Returns(figureOwnerMock.Object);

        var figureGroupMock = new Mock<IFigureGroup>();
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {1, new Uri("component/Images/test.png", UriKind.Relative)},
            {2, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        figureGroupMock.Setup(x => x.GetFigureTypeById(23)).Returns(figureTypeMock.Object);

        var figureCreator = new FigureCreator(figureOwnersMock.Object, figureGroupMock.Object);

        var figure = figureCreator.CreateFigure(new FigureBlueprint(Player.Black, 23, true));

        Assert.Equal(figureOwnerMock.Object, figure.Owner);
        Assert.Equal(figureTypeMock.Object, figure.Type);
        Assert.True(figure.IsKing);
    }

    [Fact]
    public void CreateEmptyFigure_CreatesEmptyFigure()
    {
        var figureOwnersMock = new Mock<IFigureOwnersHolder>();

        var figureGroupMock = new Mock<IFigureGroup>();
        var figureTypeMock = new Mock<IFigureType>();
        figureTypeMock.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>{
            {0, new Uri("component/Images/test.png", UriKind.Relative)},
        });
        figureGroupMock.Setup(x => x.GetFigureTypeById(0)).Returns(figureTypeMock.Object);

        var figureCreator = new FigureCreator(figureOwnersMock.Object, figureGroupMock.Object);

        var figure = figureCreator.CreateEmptyFigure();

        Assert.Equal(NeutralFigureOwner.Instance, figure.Owner);
        Assert.Equal(figureTypeMock.Object, figure.Type);
        Assert.False(figure.IsKing);
    }
}