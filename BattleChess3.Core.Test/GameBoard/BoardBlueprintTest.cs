using BattleChess3.Core.GameBoard;

namespace BattleChess3.Core.Test.GameBoard;

public class BoardBlueprintTest
{
    [Fact]
    public Task VerifyChessBoard()
    {
        return Verify(BoardBlueprint.ChessTeam);
    }
}