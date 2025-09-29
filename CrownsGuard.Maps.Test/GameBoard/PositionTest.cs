using AwesomeAssertions;
using CrownsGuard.Maps.GameBoard;

namespace CrownsGuard.Maps.Test.GameBoard;

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
        position1.Should().Be(position2);
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
        position1.Should().NotBe(position2);
    }
}