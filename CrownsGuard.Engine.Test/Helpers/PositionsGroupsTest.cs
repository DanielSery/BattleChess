using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Test.Helpers;

public class PositionsGroupsTest
{
    [Fact]
    public Task VerifyChessBoard()
    {
        return Verify(PositionsGroups.RookDirections);
    }
    
    [Fact]
    public Task BishopDirections_Verify()
    {
        return Verify(PositionsGroups.BishopDirections);
    }
    
    [Fact]
    public Task QueenDirections_Verify()
    {
        return Verify(PositionsGroups.QueenDirections);
    }
    
    [Fact]
    public Task KnightPositions_Verify()
    {
        return Verify(PositionsGroups.KnightPositions);
    }
}