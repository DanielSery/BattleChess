using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Mage
{
    private static readonly short[] MovementPositions =
    [
        unchecked((byte)-2)-2*PositionConstants.YOffset, +unchecked((byte)-2)+0*PositionConstants.YOffset, +unchecked((byte)-2)+2*PositionConstants.YOffset,
        unchecked((byte)+0)-2*PositionConstants.YOffset, unchecked((byte)+0)+2*PositionConstants.YOffset,
        unchecked((byte)+2)-2*PositionConstants.YOffset, unchecked((byte)+2)+0*PositionConstants.YOffset, unchecked((byte)+2)+2*PositionConstants.YOffset
    ];

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in MovementPositions)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.MageMove, sourceIndex, targetIndex, sourceFigure));
            }
        }
    }

    public static void ExecuteMove(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (action.FigureActionType == FigureActionType.Move)
        {
            var movement = action.TargetIndex - action.SourceIndex;
            board.MoveFigure(action.SourceIndex, action.TargetIndex, onEvent);
            if (movement % 8 == movement / 8)
            {
                TryDestroyTile(board, action.SourceIndex, unchecked((byte)+1)+0*PositionConstants.YOffset, onEvent);
                TryDestroyTile(board, action.SourceIndex, unchecked((byte)-1)+0*PositionConstants.YOffset, onEvent);
                TryDestroyTile(board, action.SourceIndex, unchecked((byte)+0)+1*PositionConstants.YOffset, onEvent);
                TryDestroyTile(board, action.SourceIndex, unchecked((byte)+0)-1*PositionConstants.YOffset, onEvent);
            }
            else
            {
                TryDestroyTile(board, action.SourceIndex, unchecked((byte)+1)-1*PositionConstants.YOffset, onEvent);
                TryDestroyTile(board, action.SourceIndex, unchecked((byte)-1)+1*PositionConstants.YOffset, onEvent);
                TryDestroyTile(board, action.SourceIndex, unchecked((byte)+1)+1*PositionConstants.YOffset, onEvent);
                TryDestroyTile(board, action.SourceIndex, unchecked((byte)-1)-1*PositionConstants.YOffset, onEvent);
            }
        }
        else
        {
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
    
    private static void TryDestroyTile(Span<Figure> board, int sourceIndex, short relative,
        Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetIndex = sourceIndex.GetWithOffset(relative);
        if (targetIndex == -1) return;

        board.KillWithoutMove((byte)sourceIndex, (byte)targetIndex, onEvent);
    }
}