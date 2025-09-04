using AwesomeAssertions;
using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.Core.Players;
using BattleChess3.Maps.BoardBlueprints;
using BattleChess3.Maps.Figures;
using Moq;

namespace BattleChess3.Maps.Test.BoardBlueprints;

public class BoardLoaderTest
{
    private readonly BoardLoader _underTest;

    public BoardLoaderTest()
    {
        var figureCreatorMock = new Mock<IFigureCreator>();
        figureCreatorMock.Setup(x => x.CreateFigure(It.IsAny<FigureBlueprint>()))
            .Returns<FigureBlueprint>(blueprint =>
            {
                var figureMock = new Mock<IFigure>();
                var figureTypeMock = new Mock<IFigureType>();
                figureTypeMock.Setup(x => x.FigureId).Returns(blueprint.FigureId);
                var ownerMock = new Mock<IFigureOwner>();
                ownerMock.Setup(x => x.Player).Returns(blueprint.Player);

                figureMock.Setup(x => x.Type).Returns(figureTypeMock.Object);
                figureMock.Setup(x => x.IsKing).Returns(blueprint.IsKing);
                figureMock.Setup(x => x.Owner).Returns(ownerMock.Object);
                return figureMock.Object;
            });

        figureCreatorMock.Setup(x => x.CreateEmptyFigure())
            .Returns(() =>
            {
                var figureMock = new Mock<IFigure>();
                var figureTypeMock = new Mock<IFigureType>();
                figureTypeMock.Setup(x => x.FigureId).Returns(0);
                var ownerMock = new Mock<IFigureOwner>();
                ownerMock.Setup(x => x.Player).Returns(Player.Neutral);

                figureMock.Setup(x => x.Type).Returns(figureTypeMock.Object);
                figureMock.Setup(x => x.IsKing).Returns(false);
                figureMock.Setup(x => x.Owner).Returns(ownerMock.Object);
                return figureMock.Object;
            });

        _underTest = new BoardLoader(figureCreatorMock.Object);
    }

    [Fact]
    public void LoadBoard_ChecksBoardDimensions()
    {
        var tileMocks = CreateTileMocks(10);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(1, 64).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);
        boardBlueprint.Figures[1] = new FigureBlueprint(Player.Black, 1, true);

        var action = () => _underTest.LoadBoard(boardMock.Object, boardBlueprint);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LoadBoard_ChecksFiguresDimensions()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(1, 10).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);
        boardBlueprint.Figures[1] = new FigureBlueprint(Player.Black, 1, true);

        var action = () => _underTest.LoadBoard(boardMock.Object, boardBlueprint);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LoadBoard_ChecksWhiteKing()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(1, 64).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[1] = new FigureBlueprint(Player.Black, 1, true);

        var action = () => _underTest.LoadBoard(boardMock.Object, boardBlueprint);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LoadBoard_ChecksBlackKing()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(1, 64).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);

        var action = () => _underTest.LoadBoard(boardMock.Object, boardBlueprint);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LoadBoard_WhenAllValid_SetsAllTiles()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(0, 64).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);
        boardBlueprint.Figures[1] = new FigureBlueprint(Player.Black, 1, true);

        _underTest.LoadBoard(boardMock.Object, boardBlueprint);

        for (var index = 0; index < tileMocks.Length; index++)
        {
            var figureId = index;
            var tileMock = tileMocks[index];
            tileMock.VerifySet(x => x.Figure = It.Is<IFigure>(figure => figure.Type.FigureId == figureId), Times.Once);
        }
    }

    [Fact]
    public void LoadMapExtendedFor2Players_ChecksBoardDimensions()
    {
        var tileMocks = CreateTileMocks(10);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(0, 16).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);

        var action = () => _underTest.LoadBoardExtendedFor2Players(boardMock.Object, boardBlueprint);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LoadMapExtendedFor2Players_ChecksFiguresDimensions()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(1, 64).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);

        var action = () => _underTest.LoadBoardExtendedFor2Players(boardMock.Object, boardBlueprint);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LoadMapExtendedFor2Players_ChecksWhiteKing()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(0, 16).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };

        var action = () => _underTest.LoadBoardExtendedFor2Players(boardMock.Object, boardBlueprint);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LoadMapExtendedFor2Players_ChecksBlackFigure()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(0, 16).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.Black, 1, false);

        var action = () => _underTest.LoadBoardExtendedFor2Players(boardMock.Object, boardBlueprint);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LoadMapExtendedFor2Players_WhenAllValid_SetsAllTiles()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(0, 16).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);

        _underTest.LoadBoardExtendedFor2Players(boardMock.Object, boardBlueprint);

        foreach (var tileMock in tileMocks)
        {
            tileMock.VerifySet(x => x.Figure = It.IsAny<IFigure>(), Times.Once);
        }
    }

    [Fact]
    public void LoadMapExtendedFor2Players_WhenAllValid_MiddleTilesAreEmpty()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(0, 16).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);

        _underTest.LoadBoardExtendedFor2Players(boardMock.Object, boardBlueprint);

        for (var i = 16; i < 48; i++)
        {
            tileMocks[i].VerifySet(x => x.Figure = It.Is<IFigure>(figure => figure.Owner.Player == Player.Neutral &&
                                                                            figure.Type.FigureId == 0 &&
                                                                            figure.IsKing == false), Times.Once);
        }
    }

    [Fact]
    public void LoadMapExtendedFor2Players_WhenAllValid_WhiteHasCorrectFigures()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(0, 16).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);

        _underTest.LoadBoardExtendedFor2Players(boardMock.Object, boardBlueprint);

        for (var i = 48; i < 63; i++)
        {
            tileMocks[i].Object.Figure.Type.FigureId.Should().Be(i - 48);
            tileMocks[i].Object.Figure.Owner.Player.Should().Be(Player.White);
        }

        tileMocks.Count(x => x.Object.Figure is { IsKing: true, Owner.Player: Player.White }).Should().Be(1);
    }

    [Fact]
    public void LoadMapExtendedFor2Players_WhenAllValid_BlackHasCorrectFigures()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(0, 16).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);

        _underTest.LoadBoardExtendedFor2Players(boardMock.Object, boardBlueprint);

        for (var i = 0; i < 8; i++)
        {
            tileMocks[i].Object.Figure.Type.FigureId.Should().Be(i + 8);
            tileMocks[i].Object.Figure.Owner.Player.Should().Be(Player.Black);
        }

        for (var i = 8; i < 15; i++)
        {
            tileMocks[i].Object.Figure.Type.FigureId.Should().Be(i - 8);
            tileMocks[i].Object.Figure.Owner.Player.Should().Be(Player.Black);
        }

        tileMocks.Count(x => x.Object.Figure is { IsKing: true, Owner.Player: Player.Black }).Should().Be(1);
    }

    [Fact]
    public void LoadTeamBoard_ChecksBoardDimensions()
    {
        var tileMocks = CreateTileMocks(10);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(1, 12).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);

        var action = () => _underTest.LoadTeamBoard(boardMock.Object, boardBlueprint);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LoadTeamBoard_ChecksWhiteKing()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(1, 64).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };

        var action = () => _underTest.LoadTeamBoard(boardMock.Object, boardBlueprint);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LoadTeamBoard_ChecksBlackFigures()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(1, 64).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.Black, 0, false);

        var action = () => _underTest.LoadTeamBoard(boardMock.Object, boardBlueprint);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LoadTeamBoard_WhenAllValid_SetsAllTiles()
    {
        var tileMocks = CreateTileMocks(64);
        var boardMock = new Mock<IBoard>();
        boardMock.Setup(x => x.GetEnumerator()).Returns(tileMocks.Select(x => x.Object).GetEnumerator);
        boardMock.Setup(x => x[It.IsAny<Position>()]).Returns<Position>(x => tileMocks[x.GetIndex()].Object);

        var boardBlueprint = new BoardBlueprint
        {
            Figures = Enumerable.Range(0, 64).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        boardBlueprint.Figures[0] = new FigureBlueprint(Player.White, 0, true);

        _underTest.LoadTeamBoard(boardMock.Object, boardBlueprint);

        for (var index = 0; index < tileMocks.Length; index++)
        {
            var figureId = index;
            var tileMock = tileMocks[index];
            tileMock.VerifySet(x => x.Figure = It.Is<IFigure>(figure => figure.Type.FigureId == figureId), Times.Once);
        }
    }

    private Mock<ITile>[] CreateTileMocks(int count)
    {
        return Enumerable.Range(1, count).Select(_ =>
        {
            var mock = new Mock<ITile>();
            mock.SetupProperty(x => x.Figure);
            return mock;
        }).ToArray();
    }
}