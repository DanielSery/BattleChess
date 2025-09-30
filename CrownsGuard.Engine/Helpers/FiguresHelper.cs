using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;

namespace CrownsGuard.Engine.Helpers;

internal static class FiguresHelper
{
    public static void CreateFigure(this Span<Figure> board, byte targetIndex, Figure createdFigure, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetFigure = board[targetIndex];
        board[targetIndex] = createdFigure;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.CreatedFigure, targetIndex, targetIndex, targetFigure), board);
    }

    public static void Die(this Span<Figure> board, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetFigure = board[toIndex];
        board[toIndex] = Figure.Empty;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toIndex, toIndex, targetFigure), board);
    }

    public static void SwapTiles(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var fromFigure = board[fromIndex];
        var toFigure = board[toIndex];
        (board[toIndex], board[fromIndex]) = (fromFigure, toFigure);
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromIndex, toIndex, toFigure), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, toIndex, fromIndex, fromFigure), board);
    }

    public static void MoveFigure(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var toFigure = board[toIndex];
        board[toIndex] = board[fromIndex];
        board[fromIndex] = Figure.Empty;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromIndex, toIndex, toFigure), board);
    }

    public static void KillWithoutMove(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var toFigure = board[toIndex];
        board[toIndex] = Figure.Empty;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toIndex, toIndex, toFigure), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacked, fromIndex, toIndex, toFigure), board);
    }

    public static void KillWithMove(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var toFigure = board[toIndex];
        board[toIndex] = board[fromIndex];
        board[fromIndex] = Figure.Empty;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacked, fromIndex, toIndex, toFigure), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toIndex, toIndex, toFigure), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromIndex, toIndex, toFigure), board);
    }

    public static void ConvertUnit(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var toFigure = board[toIndex];
        var sourceFigure = board[fromIndex];
        var targetFigure = board[toIndex];
        board[toIndex] = sourceFigure.GetFigureColor() | targetFigure.GetFigureType();
        
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedOwner, fromIndex, toIndex, toFigure), board);
    }

    public static void ChangeFigureType(this Span<Figure> board, byte fromIndex, byte toIndex, Figure figureType, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var toFigure = board[toIndex];
        var sourceFigure = board[fromIndex];
        board[toIndex] = sourceFigure.GetFigureColor() | sourceFigure.GetIsKing() | figureType;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedFigure, fromIndex, toIndex, toFigure), board);
    }

    public static void MakeUnitKing(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var toFigure = board[toIndex];
        var targetFigure = board[toIndex];
        board[toIndex] = targetFigure.GetFigureColor() | Figure.IsKing | targetFigure.GetFigureType();
        
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedFigure, fromIndex, toIndex, toFigure), board);
    }
}