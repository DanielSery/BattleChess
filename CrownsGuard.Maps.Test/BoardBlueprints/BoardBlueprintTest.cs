using CrownsGuard.Maps.BoardBlueprints;

namespace CrownsGuard.Maps.Test.BoardBlueprints;

public class BoardBlueprintTest
{
    [Fact]
    public Task VerifyChessBoard()
    {
        return Verify(BoardBlueprint.ChessTeam);
    }
}