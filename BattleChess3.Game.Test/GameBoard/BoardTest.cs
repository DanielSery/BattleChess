using BattleChess3.Game.GameBoard;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace BattleChess3.Game.Test.GameBoard;

public class BoardTest
{
    private readonly Board _halfBoard;

    public BoardTest()
    {
        var tiles = Enumerable.Range(0, 32).Select(index =>
        {
            var tile = new Mock<ITile>();
            tile.Setup(x => x.RelativePosition).Returns(Position.FromIndex(index));
            tile.Setup(x => x.AbsolutePosition).Returns(Position.FromIndex(index));
            return tile.Object;
        }).ToArray();

        _halfBoard = new Board(tiles);
    }

    [Fact]
    public void AllTilesAreInBoard()
    {
        foreach (var tile in _halfBoard)
        {
            Assert.True(_halfBoard.HasTileOnPosition(tile.RelativePosition));
        }
    }

    [Fact]
    public void AllPositionsAreRetrievableByPosition()
    {
        foreach (var tile in _halfBoard)
        {
            Assert.True(_halfBoard[tile.RelativePosition].RelativePosition == tile.RelativePosition);
        }
    }

    [Fact]
    public void AllPositionsAreRetrievableByTryGetTile()
    {
        foreach (var tile in _halfBoard)
        {
            if (_halfBoard.TryGetTile(tile.RelativePosition, out var retrievedTile))
                Assert.Equal(tile.RelativePosition, retrievedTile.RelativePosition);
            else
                Assert.Fail("All tiles should be retrievable when enumerating board");
        }
    }

    [Fact]
    public void TryTileReturnsFalseForTileYOutOfBoard()
    {
        var resultBool = _halfBoard.TryGetTile(new Position(2, 10), out var resultTile);

        Assert.False(resultBool);
        Assert.Equal(NoneTile.Instance, resultTile);
    }

    [Fact]
    public void TryTileReturnsFalseForTileXOutOfBoard()
    {
        var resultBool = _halfBoard.TryGetTile(new Position(10, 2), out var resultTile);

        Assert.False(resultBool);
        Assert.Equal(NoneTile.Instance, resultTile);
    }

    [Fact]
    public void TryTileReturnsFalseForTileOutOfBoard()
    {
        var resultBool = _halfBoard.TryGetTile(new Position(10, 10), out var resultTile);

        Assert.False(resultBool);
        Assert.Equal(NoneTile.Instance, resultTile);
    }

    public static TheoryData<Position, bool> PositionInsideData()
    {
        return new TheoryData<Position, bool>
        {
            { new Position(-1, 0), false },
            { new Position(0, -1), false },
            { new Position(0, 0), true },
            { new Position(7, 3), true },
            { new Position(7, 4), false },
            { new Position(8, 3), false },
        };
    }

    [Theory]
    [MemberData(nameof(PositionInsideData))]
    public void PositionInside(Position position, bool expected)
    {
        Assert.Equal(expected, _halfBoard.HasTileOnPosition(position));
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
        var position = Position.FromIndex(index);

        Assert.Equal(expected, _halfBoard.HasTileOnPosition(position));
    }
}