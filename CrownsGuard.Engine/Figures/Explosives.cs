using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Explosives
{
    public static void OnAttacked(BoardEvent boardEvent, Figure[] board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        TryDestroyTile(board, boardEvent.SourceIndex, PositionConstants.D1L1, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex, PositionConstants.L1, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex, PositionConstants.U1L1, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex, PositionConstants.D1, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex, PositionConstants.U1, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex, PositionConstants.D1R1, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex, PositionConstants.R1, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex, PositionConstants.U1R1, onEvent);
    }
    
    private static void TryDestroyTile(Span<Figure> board, int sourceIndex, short relative,
        Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetIndex = sourceIndex.GetWithOffset(relative);
        if (targetIndex == -1) return;

        board.KillWithoutMove((byte)sourceIndex, (byte)targetIndex, onEvent);
    }
}