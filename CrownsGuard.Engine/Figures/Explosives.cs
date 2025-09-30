using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Explosives
{
    public static void OnAttacked(BoardEvent boardEvent, Span<Figure> board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        TryDestroyTile(board, boardEvent.TargetIndex, PositionConstants.Center, onEvent);
        TryDestroyTile(board, boardEvent.TargetIndex, PositionConstants.D1L1, onEvent);
        TryDestroyTile(board, boardEvent.TargetIndex, PositionConstants.L1, onEvent);
        TryDestroyTile(board, boardEvent.TargetIndex, PositionConstants.U1L1, onEvent);
        TryDestroyTile(board, boardEvent.TargetIndex, PositionConstants.D1, onEvent);
        TryDestroyTile(board, boardEvent.TargetIndex, PositionConstants.U1, onEvent);
        TryDestroyTile(board, boardEvent.TargetIndex, PositionConstants.D1R1, onEvent);
        TryDestroyTile(board, boardEvent.TargetIndex, PositionConstants.R1, onEvent);
        TryDestroyTile(board, boardEvent.TargetIndex, PositionConstants.U1R1, onEvent);
    }
    
    private static void TryDestroyTile(Span<Figure> board, int sourceIndex, short relative,
        Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetIndex = sourceIndex.GetWithOffset(relative);
        if (targetIndex == -1) return;

        board.KillWithoutMove((byte)sourceIndex, (byte)targetIndex, onEvent);
    }
}