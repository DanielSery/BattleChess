using BattleChess3.Game.GameBoard;
using Xunit;
using Assert = Xunit.Assert;

namespace BattleChess3.Game.Test.GameBoard;

public class PositionTest
{
    public static TheoryData<Position, Position> EqualPositionsData()
    {
        return new TheoryData<Position, Position>
        {
            { new Position(2, 2), new Position(2, 2) },
            { new Position(2, 1), new Position(2, 1) }
        };
    }

    [Theory]
    [MemberData(nameof(EqualPositionsData))]
    public void EqualPosition_EqualityTrue(Position position1, Position position2)
    {
        Assert.Equal(position1, position2);
    }

    public static TheoryData<Position, Position> NotEqualPositionsData()
    {
        return new TheoryData<Position, Position>
        {
            { new Position(2, 2), new Position(2, 1) },
            { new Position(2, 2), new Position(1, 2) },
            { new Position(2, 1), new Position(1, 2) }
        };
    }

    [Theory]
    [MemberData(nameof(NotEqualPositionsData))]
    public void EqualPosition_EqualityFalse(Position position1, Position position2)
    {
        Assert.NotEqual(position1, position2);
    }

    public static TheoryData<Position, Position, Position> PositionAdditionData()
    {
        return new TheoryData<Position, Position,  Position>
        {
            { new Position(2, 2), new Position(2, 1), new Position(4, 3) },
            { new Position(2, 2), new Position(1, 2), new Position(3, 4) },
            { new Position(2, 1), new Position(1, 2), new Position(3, 3) }
        };
    }

    [Theory]
    [MemberData(nameof(PositionAdditionData))]
    public void PositionAddition(Position position1, Position position2, Position expected)
    {
        var result = position1 + position2;

        Assert.Equal(expected, result);
    }

    public static TheoryData<Position, Position, Position> PositionSubtractionData()
    {
        return new TheoryData<Position, Position,  Position>
        {
            { new Position(2, 2), new Position(2, 1), new Position(0, 1) },
            { new Position(2, 2), new Position(1, 2), new Position(1, 0) },
            { new Position(2, 1), new Position(1, 2), new Position(1, -1) }
        };
    }

    [Theory]
    [MemberData(nameof(PositionSubtractionData))]
    public void PositionSubtraction(Position position1, Position position2, Position expected)
    {
        var result = position1 - position2;

        Assert.Equal(expected, result);
    }

    public static TheoryData<Position, int, Position> PositionMultiplicationData()
    {
        return new TheoryData<Position, int,  Position>
        {
            { new Position(2, 1), 1, new Position(2, 1) },
            { new Position(2, 1), 0, new Position(0, 0) },
            { new Position(1, 3), -2, new Position(-2, -6) }
        };
    }

    [Theory]
    [MemberData(nameof(PositionMultiplicationData))]
    public void PositionMultiplication(Position position1, int coeff, Position expected)
    {
        var result = position1 * coeff;

        Assert.Equal(expected, result);
    }
}