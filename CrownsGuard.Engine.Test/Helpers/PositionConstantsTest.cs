using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Test.Helpers;

public class PositionConstantsTest
{
    [Fact]
    public Task VerifyChessBoard()
    {
        return Verify(PositionConstants.RookDirections);
    }
    
    [Fact]
    public Task BishopDirections_Verify()
    {
        return Verify(PositionConstants.BishopDirections);
    }
    
    [Fact]
    public Task QueenDirections_Verify()
    {
        return Verify(PositionConstants.QueenDirections);
    }
    
    [Fact]
    public Task KnightPositions_Verify()
    {
        return Verify(PositionConstants.KnightPositions);
    }
}