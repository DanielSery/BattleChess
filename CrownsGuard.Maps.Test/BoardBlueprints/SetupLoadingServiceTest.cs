using System.Text.Json;
using AwesomeAssertions;
using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Maps.BoardBlueprints;
using CrownsGuard.Maps.IO;
using CrownsGuard.Maps.Utilities;
using Moq;

namespace CrownsGuard.Maps.Test.BoardBlueprints;

public class SetupLoadingServiceTest
{
    private readonly Mock<IFileHandler> _fileHandlerMock = new();
    private readonly Mock<IDirectoryHandler> _directoryHandlerMock = new();

    [Fact]
    public void ctor_WhenNotHavingResources_CreatesDirectory()
    {
        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(false);

        _ = new SetupLoadingService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        _directoryHandlerMock.Verify(x => x.CreateDirectory("Resources"), Times.Once);
    }

    [Fact]
    public void ctor_WhenNotHavingMap_ReturnsChessMap()
    {
        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(false);

        var underTest = new SetupLoadingService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().BeSameAs(SampleSetup.ChessSetup);
    }

    [Fact]
    public void ctor_WhenReadFileFails_TriesToDeleteFile()
    {
        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Throws<Exception>();

        _ = new SetupLoadingService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        _fileHandlerMock.Verify(x => x.Delete("Resources\\TeamBoard.map"), Times.Once);
    }

    [Fact]
    public void ctor_WhenReadFileFails_ReturnsChessMap()
    {
        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Throws<Exception>();

        var underTest = new SetupLoadingService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().BeSameAs(SampleSetup.ChessSetup);
    }

    [Fact]
    public void ctor_WhenDeleteFails_NotThrowsException()
    {
        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Throws<Exception>();
        _fileHandlerMock.Setup(x => x.Delete("Resources\\TeamBoard.map")).Throws<Exception>();

        Action action = () => _ = new SetupLoadingService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

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

        var underTest = new SetupLoadingService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().BeSameAs(SampleSetup.ChessSetup);
    }

    [Fact]
    public void ctor_WhenDeserializationThrows_ReturnsChessBoard()
    {
        var invalidJson = "sdfjl;asdf[{";
        var text = CompressionHelper.Compress(invalidJson);


        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Returns(text);

        var underTest = new SetupLoadingService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().BeSameAs(SampleSetup.ChessSetup);
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

        var underTest = new SetupLoadingService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().BeSameAs(SampleSetup.ChessSetup);
    }

    [Fact]
    public void ctor_WhenDeserializesNoKing_ReturnsChessBoard()
    {
        var board = new BoardBlueprint
        {
            Figures = Enumerable.Range(0, 16).Select(x => (Figure)x | Figure.IsWhite).ToArray()
        };

        var serialized = JsonSerializer.Serialize(board);
        var text = CompressionHelper.Compress(serialized);

        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Returns(text);

        var underTest = new SetupLoadingService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().BeSameAs(SampleSetup.ChessSetup);
    }

    [Fact]
    public void ctor_WhenValidBoard_ReturnsBoard()
    {
        var board = new BoardBlueprint
        {
            Figures = Enumerable.Range(1, 16).Select(x => (Figure)x | Figure.IsWhite).ToArray()
        };
        board.Figures[0] = (Figure)1 | Figure.IsWhite | Figure.IsKing;

        var serialized = JsonSerializer.Serialize(board.Figures);
        var text = CompressionHelper.Compress(serialized);

        _directoryHandlerMock.Setup(x => x.Exists("Resources")).Returns(true);
        _fileHandlerMock.Setup(x => x.Exists("Resources\\TeamBoard.map")).Returns(true);
        _fileHandlerMock.Setup(x => x.ReadAllText("Resources\\TeamBoard.map")).Returns(text);

        var underTest = new SetupLoadingService(_fileHandlerMock.Object, _directoryHandlerMock.Object);

        underTest.CurrentMap.Should().HaveCount(16);
        underTest.CurrentMap.Should().ContainSingle(x => x.IsKing());
        underTest.CurrentMap.Select(x => x.GetFigureType()).Should().BeInAscendingOrder();
    }
}