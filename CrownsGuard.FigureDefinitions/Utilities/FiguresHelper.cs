using System.Runtime.CompilerServices;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Core.SimulatedBoard;

namespace CrownsGuard.FigureDefinitions.Utilities;

internal static class FiguresHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(this Figure checkedFigure)
    {
        return checkedFigure.FigureType == FigureType.Empty;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWalkable(this Figure checkedFigure)
    {
        return checkedFigure.FigureType <= FigureType.LastWalkableFigure;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanAttack(this Figure yoursFigure, Figure checkedFigure)
    {
        return yoursFigure.Player != checkedFigure.Player &&
               checkedFigure.FigureType > FigureType.LastNonAttackableFigure;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAllyTo(this Figure yoursFigure, Figure checkedFigure)
    {
        return yoursFigure.Player == checkedFigure.Player;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnemyTo(this Figure yoursFigure, Figure checkedFigure)
    {
        return yoursFigure.Player != checkedFigure.Player &&
               checkedFigure.Player != Player.Neutral;
    }

    public static void CreateFigure(this Figure[] board, Position position, Figure createdFigure, Action<BoardEvent, Figure[]> onEvent)
    {
        onEvent.Invoke(new BoardEvent(BoardEventType.Creating, position), board);
        board[position.GetIndex()] = createdFigure;
        onEvent.Invoke(new BoardEvent(BoardEventType.Created, position), board);
    }

    public static void Die(this Figure[] board, Position toPosition, Action<BoardEvent, Figure[]> onEvent)
    {
        var toIndex = toPosition.GetIndex();
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Dying, toPosition), board);
        board[toIndex] = new Figure(Player.Neutral, false, FigureType.Empty);
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toPosition), board);
    }

    public static void SwapTiles(this Figure[] board, Position fromPosition, Position toPosition, Action<BoardEvent, Figure[]> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moving, fromPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Moving, toPosition), board);
        
        (board[toIndex], board[fromIndex]) = (board[fromIndex], board[toIndex]);
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, fromPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, toPosition), board);
    }

    public static void MoveFigure(this Figure[] board, Position fromPosition, Position toPosition, Action<BoardEvent, Figure[]> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moving, fromPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Dying, toPosition), board);
        
        board[toIndex] = board[fromIndex];
        board[fromIndex] = new Figure(Player.Neutral, false, FigureType.Empty);
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moved, toPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toPosition), board);
    }

    public static void KillWithoutMove(this Figure[] board, Position fromPosition, Position toPosition, Action<BoardEvent, Figure[]> onEvent)
    {
        var toIndex = toPosition.GetIndex();
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacking, fromPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Dying, toPosition), board);

        board[toIndex] = new Figure(Player.Neutral, false, FigureType.Empty);
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacked, fromPosition), board);
    }

    public static void KillWithMove(this Figure[] board, Position fromPosition, Position toPosition, Action<BoardEvent, Figure[]> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moving, fromPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacking, fromPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Dying, toPosition), board);

        board[toIndex] = board[fromIndex];
        board[fromIndex] = new Figure(Player.Neutral, false, FigureType.Empty);
        
        onEvent.Invoke(new BoardEvent(BoardEventType.Moving, toPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Attacked, fromPosition), board);
        onEvent.Invoke(new BoardEvent(BoardEventType.Died, toPosition), board);
    }

    public static void ChangeOwner(this Figure[] board, Position fromPosition, Position toPosition, Action<BoardEvent, Figure[]> onEvent)
    {
        var fromIndex = fromPosition.GetIndex();
        var toIndex = toPosition.GetIndex();
        
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangingOwner, fromPosition), board);
        var sourceFigure = board[fromIndex];
        board[toIndex] = new Figure(sourceFigure.Player, false, sourceFigure.FigureType);
        onEvent.Invoke(new BoardEvent(BoardEventType.ChangedOwner, fromPosition), board);
    }
}