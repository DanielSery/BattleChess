using CrownsGuard.Core.Figures;
using CrownsGuard.Maps.BoardBlueprints;
using CrownsGuard.Maps.Figures;
using CrownsGuard.Maps.GameBoard;
using Moq;

namespace CrownsGuard.Maps.Test.BoardBlueprints
{
    public class BoardLoaderTest
    {
        private static Mock<IFigureCreator> CreateFigureCreatorMock()
        {
            var mock = new Mock<IFigureCreator>();
            var dummyFigure = new Mock<IFigureWithInfo>().Object;
            mock.Setup(f => f.CreateFigure(It.IsAny<Figure>())).Returns(dummyFigure);
            return mock;
        }

        private static IBoardInfo CreateTiles(int count)
        {
            var tiles = new List<ITileInfo>();
            for (int i = 0; i < count; i++)
            {
                var tileMock = new Mock<ITileInfo>();
                tileMock.SetupProperty(t => t.Figure);
                tiles.Add(tileMock.Object);
            }
            return new BoardInfo(tiles.ToArray());
        }

        private static BoardBlueprint CreateValidBlueprint(int count)
        {
            var figures = new Figure[count];
            figures[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
            figures[1] = Figure.King | Figure.IsBlack | Figure.IsKing;
            for (int i = 2; i < count; i++)
                figures[i] = Figure.LegionarySword | Figure.IsWhite;
            return new BoardBlueprint { Figures = figures };
        }

        private static BoardBlueprint CreateValidTeamBlueprint(int count)
        {
            var figures = new Figure[count];
            figures[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
            for (int i = 2; i < count; i++)
                figures[i] = Figure.LegionarySword | Figure.IsWhite;
            return new BoardBlueprint { Figures = figures };
        }

        [Fact]
        public void LoadBoard_WithInvalidBlueprintSize_Throws()
        {
            var figureCreator = CreateFigureCreatorMock();
            var loader = new BoardLoader(figureCreator.Object);
            var tiles = CreateTiles(64);
            var blueprint = CreateValidBlueprint(65);

            Assert.Throws<ArgumentException>(() => loader.LoadBoard(tiles, blueprint));
        }

        [Fact]
        public void LoadBoard_AssignsFiguresToAllTiles()
        {
            var figureCreator = CreateFigureCreatorMock();
            var loader = new BoardLoader(figureCreator.Object);
            var tiles = CreateTiles(64);
            var blueprint = CreateValidBlueprint(64);

            loader.LoadBoard(tiles, blueprint);

            foreach (var tile in tiles)
                Assert.NotNull(tile.Figure);
            figureCreator.Verify(f => f.CreateFigure(It.IsAny<Figure>()), Times.Exactly(64));
        }

        [Fact]
        public void LoadBoard_ThrowsIfBoardInfoCountNot64()
        {
            var loader = new BoardLoader(CreateFigureCreatorMock().Object);
            var tiles = CreateTiles(63);
            var blueprint = CreateValidBlueprint(64);

            Assert.Throws<ArgumentException>(() => loader.LoadBoard(tiles, blueprint));
        }

        [Fact]
        public void LoadBoard_ThrowsIfBlueprintFiguresLengthNot64()
        {
            var loader = new BoardLoader(CreateFigureCreatorMock().Object);
            var tiles = CreateTiles(64);
            var blueprint = CreateValidBlueprint(63);

            Assert.Throws<ArgumentException>(() => loader.LoadBoard(tiles, blueprint));
        }

        [Fact]
        public void LoadBoard_ThrowsIfNotExactlyOneWhiteKing()
        {
            var loader = new BoardLoader(CreateFigureCreatorMock().Object);
            var tiles = CreateTiles(64);
            var blueprint = CreateValidBlueprint(64);
            blueprint.Figures[0] = Figure.LegionarySword | Figure.IsWhite; // Remove white king

            Assert.Throws<ArgumentException>(() => loader.LoadBoard(tiles, blueprint));
        }

        [Fact]
        public void LoadBoard_ThrowsIfNotExactlyOneBlackKing()
        {
            var loader = new BoardLoader(CreateFigureCreatorMock().Object);
            var tiles = CreateTiles(64);
            var blueprint = CreateValidBlueprint(64);
            blueprint.Figures[1] = Figure.LegionarySword | Figure.IsBlack; // Remove black king

            Assert.Throws<ArgumentException>(() => loader.LoadBoard(tiles, blueprint));
        }

        [Fact]
        public void LoadTeamBoard_ThrowsIfBoardInfoCountNot16()
        {
            var loader = new BoardLoader(CreateFigureCreatorMock().Object);
            var tiles = CreateTiles(15);
            var blueprint = CreateValidTeamBlueprint(64);

            Assert.Throws<ArgumentException>(() => loader.LoadTeamBoard(tiles, blueprint));
        }

        [Fact]
        public void LoadTeamBoard_ThrowsIfBlueprintFiguresLengthNot16()
        {
            var loader = new BoardLoader(CreateFigureCreatorMock().Object);
            var tiles = CreateTiles(16);
            var blueprint = CreateValidTeamBlueprint(15);

            Assert.Throws<ArgumentException>(() => loader.LoadTeamBoard(tiles, blueprint));
        }

        [Fact]
        public void LoadTeamBoard_ThrowsIfNotExactlyOneWhiteKing()
        {
            var loader = new BoardLoader(CreateFigureCreatorMock().Object);
            var tiles = CreateTiles(16);
            var blueprint = CreateValidTeamBlueprint(64);
            blueprint.Figures[0] = Figure.LegionarySword | Figure.IsWhite; // Remove white king

            Assert.Throws<ArgumentException>(() => loader.LoadTeamBoard(tiles, blueprint));
        }

        [Fact]
        public void LoadTeamBoard_ThrowsIfAnyBlackFigurePresent()
        {
            var loader = new BoardLoader(CreateFigureCreatorMock().Object);
            var tiles = CreateTiles(16);
            var blueprint = CreateValidTeamBlueprint(16);
            blueprint.Figures[5] = Figure.LegionarySword | Figure.IsBlack;

            Assert.Throws<ArgumentException>(() => loader.LoadTeamBoard(tiles, blueprint));
        }

        [Fact]
        public void LoadTeamBoard_AssignsFiguresToAllTiles()
        {
            var figureCreator = CreateFigureCreatorMock();
            var loader = new BoardLoader(figureCreator.Object);
            var tiles = CreateTiles(16);
            var blueprint = CreateValidTeamBlueprint(16);

            loader.LoadTeamBoard(tiles, blueprint);

            foreach (var tile in tiles)
                Assert.NotNull(tile.Figure);
            figureCreator.Verify(f => f.CreateFigure(It.IsAny<Figure>()), Times.Exactly(16));
        }
    }
}
