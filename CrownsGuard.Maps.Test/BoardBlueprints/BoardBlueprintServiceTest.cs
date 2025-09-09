using System.Text.Json;
using AwesomeAssertions;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Maps.BoardBlueprints;
using CrownsGuard.Maps.IO;
using CrownsGuard.Maps.Utilities;
using Moq;

namespace CrownsGuard.Maps.Test.BoardBlueprints;

public class BoardBlueprintServiceTest
{
    private readonly Mock<IFileHandler> _fileHandlerMock;
    private readonly Mock<IDirectoryHandler> _directoryHandlerMock;

    public BoardBlueprintServiceTest()
    {
        _fileHandlerMock = new Mock<IFileHandler>();
        _directoryHandlerMock = new Mock<IDirectoryHandler>();
    }

    [Fact]
    public void ctor_WhenNotHavingResources_CreatesDirectory()
    {
        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(false);

        _ = new BoardBlueprintService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        _directoryHandlerMock.Verify(x => x.CreateDirectory("Resources"), Times.Once);
    }

    [Fact]
    public void ctor_WhenNotHavingMap_ReturnsChessMap()
    {
        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(false);

        var underTest = new BoardBlueprintService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().Be(BoardBlueprint.ChessTeam);
    }

    [Fact]
    public void ctor_WhenReadFileFails_TriesToDeleteFile()
    {
        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Throws<Exception>();

        _ = new BoardBlueprintService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        _fileHandlerMock.Verify(x => x.Delete("Resources\\TeamBoard.map"), Times.Once);
    }

    [Fact]
    public void ctor_WhenReadFileFails_ReturnsChessMap()
    {
        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Throws<Exception>();

        var underTest = new BoardBlueprintService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().Be(BoardBlueprint.ChessTeam);
    }

    [Fact]
    public void ctor_WhenDeleteFails_NotThrowsException()
    {
        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Throws<Exception>();
        _fileHandlerMock.Setup(x => x.Delete("Resources\\TeamBoard.map")).Throws<Exception>();

        Action action = () => _ = new BoardBlueprintService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        action.Should().NotThrow();
    }

    [Fact]
    public void ctor_WhenDeserializationReturnEmptyMap_ReturnsChessBoard()
    {
        BoardBlueprint? nullBoard = null;
        var text = JsonSerializer.Serialize(nullBoard);
        text = CompressionHelper.Compress(text);

        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Returns(text);

        var underTest = new BoardBlueprintService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().Be(BoardBlueprint.ChessTeam);
    }

    [Fact]
    public void ctor_WhenDeserializationThrows_ReturnsChessBoard()
    {
        var invalidJson = "sdfjl;asdf[{";
        var text = CompressionHelper.Compress(invalidJson);


        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Returns(text);

        var underTest = new BoardBlueprintService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().Be(BoardBlueprint.ChessTeam);
    }

    [Fact]
    public void ctor_WhenDeserializesInvalidBoardSize_ReturnsChessBoard()
    {
        var board = new BoardBlueprint();
        var serialized = JsonSerializer.Serialize(board);
        var text = CompressionHelper.Compress(serialized);

        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Returns(text);

        var underTest = new BoardBlueprintService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().Be(BoardBlueprint.ChessTeam);
    }

    [Fact]
    public void ctor_WhenDeserializesNoKing_ReturnsChessBoard()
    {
        var board = new BoardBlueprint
        {
            Figures = Enumerable.Range(0, 16).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };

        var serialized = JsonSerializer.Serialize(board);
        var text = CompressionHelper.Compress(serialized);

        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Returns(text);

        var underTest = new BoardBlueprintService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().Be(BoardBlueprint.ChessTeam);
    }

    [Fact]
    public void ctor_WhenValidBoard_ReturnsBoard()
    {
        var board = new BoardBlueprint
        {
            Figures = Enumerable.Range(1, 16).Select(x => new FigureBlueprint(Player.White, x, false)).ToArray()
        };
        board.Figures[0] = new FigureBlueprint(Player.White, 1, true);

        var serialized = JsonSerializer.Serialize(board);
        var text = CompressionHelper.Compress(serialized);

        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Returns(text);

        var underTest = new BoardBlueprintService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Figures.Should().HaveCount(16);
        underTest.CurrentMap.Figures.Should().ContainSingle(x => x.IsKing);
        underTest.CurrentMap.Figures.Select(x => x.FigureType).Should().BeInAscendingOrder();
    }
}