using System.Runtime.CompilerServices;
using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;

namespace CrownsGuard.Engine.Helpers;

internal static class FiguresHelper
{
    public static void CreateFigure(this Span<Figure> board, byte targetIndex, Figure createdFigure, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board[targetIndex] = createdFigure;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.CreatedFigure, targetIndex, targetIndex), board);
    }

    public static void Die(this Span<Figure> board, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board[toIndex] = Figure.Empty;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toIndex, toIndex), board);
    }

    public static void SwapTiles(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        (board[toIndex], board[fromIndex]) = (board[fromIndex], board[toIndex]);
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromIndex, toIndex), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, toIndex, fromIndex), board);
    }

    public static void MoveFigure(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board[toIndex] = board[fromIndex];
        board[fromIndex] = Figure.Empty;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromIndex, toIndex), board);
    }

    public static void KillWithoutMove(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board[toIndex] = Figure.Empty;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toIndex, toIndex), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacked, fromIndex, toIndex), board);
    }

    public static void KillWithMove(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board[toIndex] = board[fromIndex];
        board[fromIndex] = Figure.Empty;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacked, fromIndex, toIndex), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toIndex, toIndex), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromIndex, toIndex), board);
    }

    public static void ConvertUnit(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var sourceFigure = board[fromIndex];
        var targetFigure = board[toIndex];
        board[toIndex] = sourceFigure.GetFigureColor() | targetFigure.GetFigureType();
        
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedOwner, fromIndex, toIndex), board);
    }

    public static void ChangeFigureType(this Span<Figure> board, byte fromIndex, byte toIndex, Figure figureType, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var sourceFigure = board[fromIndex];
        board[toIndex] = sourceFigure.GetFigureColor() | sourceFigure.GetIsKing() | figureType;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedFigure, fromIndex, toIndex), board);
    }

    public static void MakeUnitKing(this Span<Figure> board, byte fromIndex, byte toIndex, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetFigure = board[toIndex];
        board[toIndex] = targetFigure.GetFigureColor() | Figure.IsKing | targetFigure.GetFigureType();
        
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedFigure, fromIndex, toIndex), board);
    }
}