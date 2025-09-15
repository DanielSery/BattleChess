using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;

namespace CrownsGuard.FigureDefinitions.Utilities;

internal static class FiguresHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(this Figure checkedFigure)
    {
        return checkedFigure == Figure.Empty;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWalkable(this Figure checkedFigure)
    {
        return (checkedFigure & Figure.FigureMask) <= Figure.LastWalkableFigure;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanAttack(this Figure yoursFigure, Figure checkedFigure)
    {
        return ((yoursFigure ^ checkedFigure) & Figure.PlayerMask) != Figure.Empty &&
               (checkedFigure & Figure.FigureMask) > Figure.LastNonAttackableFigure;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAllyTo(this Figure yoursFigure, Figure checkedFigure)
    {
        return ((yoursFigure ^ checkedFigure) & Figure.PlayerMask) == Figure.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnemyTo(this Figure yoursFigure, Figure checkedFigure)
    {
        return ((yoursFigure ^ checkedFigure) & Figure.PlayerMask) == Figure.PlayerMask;
    }

    public static void CreateFigure(this Span<Figure> board, Position position, Figure createdFigure, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board[position.GetIndex()] = createdFigure;
        onEvent.Invoke(new BoardEvent(BoardEventType.CreatedFigure, position, position), board);
    }

    public static void Die(this Span<Figure> board, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var toIndex = toPosition.GetIndex();
        
        board[toIndex] = Figure.Empty;
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toPosition, toPosition), board);
    }

    public static void SwapTiles(this Span<Figure> board, Position fromPosition, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();
        
        (board[toIndex], board[fromIndex]) = (board[fromIndex], board[toIndex]);
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromPosition, toPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, toPosition, fromPosition), board);
    }

    public static void MoveFigure(this Span<Figure> board, Position fromPosition, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();
        
        board[toIndex] = board[fromIndex];
        board[fromIndex] = Figure.Empty;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromPosition, toPosition), board);
    }

    public static void KillWithoutMove(this Span<Figure> board, Position fromPosition, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var toIndex = toPosition.GetIndex();
        
        board[toIndex] = Figure.Empty;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toPosition, toPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacked, fromPosition, toPosition), board);
    }

    public static void KillWithMove(this Span<Figure> board, Position fromPosition, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();
        
        board[toIndex] = board[fromIndex];
        board[fromIndex] = Figure.Empty;
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacked, fromPosition, toPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toPosition, toPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromPosition, toPosition), board);
    }

    public static void ConvertUnit(this Span<Figure> board, Position fromPosition, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var toIndex = toPosition.GetIndex();
        
        var targetFigure = board[toIndex];
        board[toIndex] = targetFigure.GetFigureColor() | targetFigure.GetFigureType();
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedOwner, fromPosition, toPosition), board);
    }

    public static void ChangeFigureType(this Span<Figure> board, Position fromPosition, Position toPosition, Figure figureType, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();

        var sourceFigure = board[fromIndex];
        board[toIndex] = sourceFigure.GetFigureColor() | sourceFigure.GetIsKing() | figureType;
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedFigure, fromPosition, toPosition), board);
    }

    public static void MakeUnitKing(this Span<Figure> board, Position fromPosition, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();

        var sourceFigure = board[fromIndex];
        board[toIndex] = sourceFigure.GetFigureColor() | Figure.IsKing | sourceFigure.GetFigureType();
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedFigure, fromPosition, toPosition), board);
    }
}