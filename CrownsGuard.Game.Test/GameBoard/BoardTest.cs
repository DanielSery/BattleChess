using Moq;

namespace CrownsGuard.Game.Test.GameBoard;

public class BoardTest
{
    private readonly Board _halfBoard;

    public BoardTest()
    {
        var tiles = Enumerable.Range(0, 32).Select(index =>
        {
            var tile = new Mock<ITile>();
            tile.Setup(x => x.RelativePosition).Returns(Position.Position.FromIndex(index));
            tile.Setup(x => x.Position).Returns(Position.Position.FromIndex(index));
            return tile.Object;
        }).ToArray();

        _halfBoard = new Board(tiles);
    }

    [Fact]
    public void AllTilesAreInBoard()
    {
        foreach (var tile in _halfBoard)
        {
            var hasTileOnPosition = _halfBoard.HasTileOnPosition(tile.RelativePosition);
            hasTileOnPosition.Should().BeTrue();
        }
    }

    [Fact]
    public void AllPositionsAreRetrievableByPosition()
    {
        foreach (var tile in _halfBoard)
        {
            var relativePosition = _halfBoard[tile.RelativePosition].RelativePosition;
            relativePosition.Should().Be(tile.RelativePosition);
        }
    }

    [Fact]
    public void AllPositionsAreRetrievableByTryGetTile()
    {
        foreach (var tile in _halfBoard)
        {
            if (_halfBoard.TryGetTile(tile.RelativePosition, out var retrievedTile))
            {
                var relativePosition = retrievedTile.RelativePosition;
                relativePosition.Should().Be(tile.RelativePosition);
            }
            else
                Assert.Fail("All tiles should be retrievable when enumerating board");
        }
    }

    [Fact]
    public void TryTileReturnsFalseForTileYOutOfBoard()
    {
        var resultBool = _halfBoard.TryGetTile(new Position.Position(2, 10), out var resultTile);

        resultBool.Should().BeFalse();
        resultTile.Should().Be(NoneTile.Instance);
    }

    [Fact]
    public void TryTileReturnsFalseForTileXOutOfBoard()
    {
        var resultBool = _halfBoard.TryGetTile(new Position.Position(10, 2), out var resultTile);

        resultBool.Should().BeFalse();
        resultTile.Should().Be(NoneTile.Instance);
    }

    [Fact]
    public void TryTileReturnsFalseForTileOutOfBoard()
    {
        var resultBool = _halfBoard.TryGetTile(new Position.Position(10, 10), out var resultTile);

        resultBool.Should().BeFalse();
        resultTile.Should().Be(NoneTile.Instance);
    }

    public static TheoryData<Position.Position, bool> PositionInsideData()
    {
        return new TheoryData<Position.Position, bool>
        {
            { new Position.Position(-1, 0), false },
            { new Position.Position(0, -1), false },
            { new Position.Position(0, 0), true },
            { new Position.Position(7, 3), true },
            { new Position.Position(7, 4), false },
            { new Position.Position(8, 3), false },
        };
    }

    [Theory]
    [MemberData(nameof(PositionInsideData))]
    public void PositionInside(Position.Position position, bool expected)
    {
        var hasTileOnPosition = _halfBoard.HasTileOnPosition(position);

        hasTileOnPosition.Should().Be(expected);
    }

    public static TheoryData<int, bool> IndexInsideData()
    {
        return new TheoryData<int, bool>
        {
            { -2, false },
            { -1, false },
            { 0, true },
            { 1, true },
            { 7, true },
            { 8, true },
            { 9, true },
            { 31, true },
            { 32, false },
            { 33, false },
            { 40, false },
            { 100, false },
        };
    }

    [Theory]
    [MemberData(nameof(IndexInsideData))]
    public void IndexInside(int index, bool expected)
    {
        var position = Position.Position.FromIndex(index);

        var hasTileOnPosition = _halfBoard.HasTileOnPosition(position);

        hasTileOnPosition.Should().Be(expected);
    }
}