using CrownsGuard.Core.Figures;
using CrownsGuard.FiguresDesign;
using CrownsGuard.FiguresDesign.Figures;
using CrownsGuard.Game;
using CrownsGuard.Game.Players;
using CrownsGuard.Maps.Figures;
using Moq;

namespace CrownsGuard.Maps.Test.Figures
{
    public class FigureCreatorTest
    {
        [Fact]
        public void CreateFigure_ReturnsCorrectFigureWithInfo_ForWhiteFigure()
        {
            var mockOwners = new Mock<IPlayersOwner>();
            var mockTypeGroup = new Mock<IFigureTypeInfoGroup>();
            var mockPlayer = new Mock<IPlayer>();
            var mockTypeInfo = new Mock<IFigureTypeInfo>();
            mockTypeInfo.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>
            {
                { 0, new Uri("https://example.com/uri0.png") },
                { 1, new Uri("https://example.com/uri1.png") },
                { 2, new Uri("https://example.com/uri2.png") }
            });

            mockPlayer.SetupGet(p => p.PlayerColor).Returns(PlayerColor.White);
            mockOwners.Setup(o => o.GetPlayer(PlayerColor.White)).Returns(mockPlayer.Object);
            mockTypeGroup.Setup(g => g.GetFigureTypeById(It.IsAny<Figure>())).Returns(mockTypeInfo.Object);

            var creator = new FigureCreator(mockOwners.Object, mockTypeGroup.Object);

            var whiteFigure = (Figure)((ushort)Figure.Fire | (ushort)Figure.IsWhite);
            var blueprint = whiteFigure;

            var result = creator.CreateFigure(blueprint);

            Assert.Equal(mockPlayer.Object, result.Owner);
            Assert.Equal(mockTypeInfo.Object, result.TypeInfo);
            Assert.False(result.IsKing);
        }

        [Fact]
        public void CreateFigure_ReturnsCorrectFigureWithInfo_ForBlackFigure()
        {
            var mockOwners = new Mock<IPlayersOwner>();
            var mockTypeGroup = new Mock<IFigureTypeInfoGroup>();
            var mockPlayer = new Mock<IPlayer>();
            var mockTypeInfo = new Mock<IFigureTypeInfo>();
            mockTypeInfo.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>
            {
                { 0, new Uri("https://example.com/uri0.png") },
                { 1, new Uri("https://example.com/uri1.png") },
                { 2, new Uri("https://example.com/uri2.png") }
            });

            mockPlayer.SetupGet(p => p.PlayerColor).Returns(PlayerColor.Black);
            mockOwners.Setup(o => o.GetPlayer(PlayerColor.Black)).Returns(mockPlayer.Object);
            mockTypeGroup.Setup(g => g.GetFigureTypeById(It.IsAny<Figure>())).Returns(mockTypeInfo.Object);

            var creator = new FigureCreator(mockOwners.Object, mockTypeGroup.Object);

            var blackFigure = (Figure)((ushort)Figure.Fire | (ushort)Figure.IsBlack);
            var blueprint = blackFigure;

            var result = creator.CreateFigure(blueprint);

            Assert.Equal(mockPlayer.Object, result.Owner);
            Assert.Equal(mockTypeInfo.Object, result.TypeInfo);
            Assert.False(result.IsKing);
        }

        [Fact]
        public void CreateFigure_ReturnsCorrectFigureWithInfo_ForNeutralFigure()
        {
            var mockOwners = new Mock<IPlayersOwner>();
            var mockTypeGroup = new Mock<IFigureTypeInfoGroup>();
            var mockPlayer = new Mock<IPlayer>();
            var mockTypeInfo = new Mock<IFigureTypeInfo>();
            mockTypeInfo.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>
            {
                { 0, new Uri("https://example.com/uri0.png") },
                { 1, new Uri("https://example.com/uri1.png") },
                { 2, new Uri("https://example.com/uri2.png") }
            });

            mockPlayer.SetupGet(p => p.PlayerColor).Returns(PlayerColor.Neutral);
            mockOwners.Setup(o => o.GetPlayer(PlayerColor.Neutral)).Returns(mockPlayer.Object);
            mockTypeGroup.Setup(g => g.GetFigureTypeById(It.IsAny<Figure>())).Returns(mockTypeInfo.Object);

            var creator = new FigureCreator(mockOwners.Object, mockTypeGroup.Object);

            var neutralFigure = Figure.Fire;
            var blueprint = neutralFigure;

            var result = creator.CreateFigure(blueprint);

            Assert.Equal(mockPlayer.Object, result.Owner);
            Assert.Equal(mockTypeInfo.Object, result.TypeInfo);
            Assert.False(result.IsKing);
        }

        [Fact]
        public void CreateFigure_SetsIsKingTrue_WhenKingFlagIsSet()
        {
            var mockOwners = new Mock<IPlayersOwner>();
            var mockTypeGroup = new Mock<IFigureTypeInfoGroup>();
            var mockPlayer = new Mock<IPlayer>();
            var mockTypeInfo = new Mock<IFigureTypeInfo>();
            mockTypeInfo.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>
            {
                { 0, new Uri("https://example.com/uri0.png") },
                { 1, new Uri("https://example.com/uri1.png") },
                { 2, new Uri("https://example.com/uri2.png") }
            });

            mockPlayer.SetupGet(p => p.PlayerColor).Returns(PlayerColor.White);
            mockOwners.Setup(o => o.GetPlayer(PlayerColor.White)).Returns(mockPlayer.Object);
            mockTypeGroup.Setup(g => g.GetFigureTypeById(It.IsAny<Figure>())).Returns(mockTypeInfo.Object);

            var creator = new FigureCreator(mockOwners.Object, mockTypeGroup.Object);

            var kingFigure = (Figure)((ushort)Figure.Fire | (ushort)Figure.IsWhite | (ushort)Figure.IsKing);
            var blueprint = kingFigure;

            var result = creator.CreateFigure(blueprint);

            Assert.Equal(mockPlayer.Object, result.Owner);
            Assert.Equal(mockTypeInfo.Object, result.TypeInfo);
            Assert.True(result.IsKing);
        }

        [Fact]
        public void CreateEmptyFigure_ReturnsNeutralPlayerAndEmptyTypeInfo()
        {
            var mockTypeGroup = new Mock<IFigureTypeInfoGroup>();
            var mockTypeInfo = new Mock<IFigureTypeInfo>();
            mockTypeInfo.Setup(x => x.ImageUris).Returns(new Dictionary<int, Uri>
            {
                { 0, new Uri("https://example.com/uri0.png") },
                { 1, new Uri("https://example.com/uri1.png") },
                { 2, new Uri("https://example.com/uri2.png") }
            });

            mockTypeGroup.Setup(g => g.GetFigureTypeById(0)).Returns(mockTypeInfo.Object);

            var creator = new FigureCreator(Mock.Of<IPlayersOwner>(), mockTypeGroup.Object);

            var result = creator.CreateEmptyFigure();

            Assert.Equal(NeutralPlayer.Instance, result.Owner);
            Assert.Equal(mockTypeInfo.Object, result.TypeInfo);
            Assert.False(result.IsKing);
        }
    }
}
