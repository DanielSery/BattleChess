using CrownsGuard.Maps.BoardBlueprints;

namespace CrownsGuard.Core.Test.GameBoard;

public class BoardBlueprintTest
{
    [Fact]
    public Task VerifyChessBoard()
    {
        return Verify(BoardBlueprint.ChessTeam);
    }
}