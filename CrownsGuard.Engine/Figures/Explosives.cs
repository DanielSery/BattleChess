using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Explosives
{
    public static void OnAttacked(BoardEvent boardEvent, Figure[] board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        TryDestroyTile(board, boardEvent.SourceIndex,  unchecked((byte)-1)-1*PositionConstants.YOffset, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  +unchecked((byte)-1)+0*PositionConstants.YOffset, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  +unchecked((byte)-1)+1*PositionConstants.YOffset, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  unchecked((byte)+0)-1*PositionConstants.YOffset, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  unchecked((byte)+0)+1*PositionConstants.YOffset, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  unchecked((byte)+1)-1*PositionConstants.YOffset, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  unchecked((byte)+1)+0*PositionConstants.YOffset, onEvent);
        TryDestroyTile(board, boardEvent.SourceIndex,  unchecked((byte)+1)+1*PositionConstants.YOffset, onEvent);
    }
    
    private static void TryDestroyTile(Span<Figure> board, int sourceIndex, short relative,
        Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetIndex = sourceIndex.GetWithOffset(relative);
        if (targetIndex == -1) return;

        board.KillWithoutMove((byte)sourceIndex, (byte)targetIndex, onEvent);
    }
}