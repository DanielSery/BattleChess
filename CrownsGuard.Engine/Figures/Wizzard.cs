using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Wizzard
{
    private static readonly short[] MovementPositions =
    [
        PositionConstants.D2L2, PositionConstants.L2, PositionConstants.U2L2,
        PositionConstants.D2, PositionConstants.U2,
        PositionConstants.D2R2, PositionConstants.R2, PositionConstants.U2R2
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
                actions.Push(new FigureAction(FigureActionType.WizzardMove, sourceIndex, targetIndex, sourceFigure));
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
                TryDestroyTile(board, action.SourceIndex, PositionConstants.R1, onEvent);
                TryDestroyTile(board, action.SourceIndex, PositionConstants.L1, onEvent);
                TryDestroyTile(board, action.SourceIndex, PositionConstants.U1, onEvent);
                TryDestroyTile(board, action.SourceIndex, PositionConstants.D1, onEvent);
            }
            else
            {
                TryDestroyTile(board, action.SourceIndex, PositionConstants.D1R1, onEvent);
                TryDestroyTile(board, action.SourceIndex, PositionConstants.U1L1, onEvent);
                TryDestroyTile(board, action.SourceIndex, PositionConstants.U1R1, onEvent);
                TryDestroyTile(board, action.SourceIndex, PositionConstants.D1L1, onEvent);
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