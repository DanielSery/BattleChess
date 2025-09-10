using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;

namespace CrownsGuard.FigureDefinitions.Utilities;

internal static class FiguresHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(this Figure checkedFigure)
    {
        return checkedFigure.FigureType == FigureId.Empty;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWalkable(this Figure checkedFigure)
    {
        return checkedFigure.FigureType <= FigureId.LastWalkableFigure;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanAttack(this Figure yoursFigure, Figure checkedFigure)
    {
        return yoursFigure.PlayerColor != checkedFigure.PlayerColor &&
               checkedFigure.FigureType > FigureId.LastNonAttackableFigure;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAllyTo(this Figure yoursFigure, Figure checkedFigure)
    {
        return yoursFigure.PlayerColor == checkedFigure.PlayerColor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnemyTo(this Figure yoursFigure, Figure checkedFigure)
    {
        return yoursFigure.PlayerColor != checkedFigure.PlayerColor &&
               checkedFigure.PlayerColor != PlayerColor.Neutral;
    }

    public static void CreateFigure(this Span<Figure> board, Position position, Figure createdFigure, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board[position.GetIndex()] = createdFigure;
        onEvent.Invoke(new BoardEvent(BoardEventType.CreatedFigure, position, position), board);
    }

    public static void Die(this Span<Figure> board, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var toIndex = toPosition.GetIndex();
        
        board[toIndex] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
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
        board[fromIndex] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromPosition, toPosition), board);
    }

    public static void KillWithoutMove(this Span<Figure> board, Position fromPosition, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var toIndex = toPosition.GetIndex();
        
        board[toIndex] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toPosition, toPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacked, fromPosition, toPosition), board);
    }

    public static void KillWithMove(this Span<Figure> board, Position fromPosition, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();
        
        board[toIndex] = board[fromIndex];
        board[fromIndex] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacked, fromPosition, toPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toPosition, toPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromPosition, toPosition), board);
    }

    public static void ChangeOwner(this Span<Figure> board, Position fromPosition, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();
        
        var sourceFigure = board[fromIndex];
        board[toIndex] = new Figure(sourceFigure.PlayerColor, false, sourceFigure.FigureType);
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedOwner, fromPosition, toPosition), board);
    }

    public static void ChangeFigureType(this Span<Figure> board, Position fromPosition, Position toPosition, FigureId figureType, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();

        var sourceFigure = board[fromIndex];
        board[toIndex] = new Figure(sourceFigure.PlayerColor, sourceFigure.IsKing, figureType);
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedFigure, fromPosition, toPosition), board);
    }

    public static void MakeUnitKing(this Span<Figure> board, Position fromPosition, Position toPosition, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();

        var sourceFigure = board[fromIndex];
        board[toIndex] = new Figure(sourceFigure.PlayerColor, true, sourceFigure.FigureType);
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedFigure, fromPosition, toPosition), board);
    }
}