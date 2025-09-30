using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Peasant
{
    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        if (sourceFigure.IsBlack())
        {
            TryAddMoveAction(board, sourceIndex, sourceFigure, PositionConstants.U1, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, PositionConstants.U1, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, PositionConstants.L1, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, PositionConstants.R1, actions);
        }
        else
        {
            TryAddMoveAction(board, sourceIndex, sourceFigure, PositionConstants.D1, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, PositionConstants.D1, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, PositionConstants.L1, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, PositionConstants.R1, actions);
        }
    }

    private static void TryAddAttackAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, short relativePosition,
        Stack<FigureAction> actions)
    {
        var targetIndex = sourceIndex.GetWithOffset(relativePosition);
        if (targetIndex == -1) return;
        var targetFigure = board[targetIndex];
        
        if (!sourceFigure.CanAttack(targetFigure))
        {
            actions.Push(new FigureAction(FigureActionType.PossibleMeleeAttack, sourceIndex, targetIndex, sourceFigure));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.MeleeAttack, sourceIndex, targetIndex, sourceFigure));
    }

    private static void TryAddMoveAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, short relativePosition,
        Stack<FigureAction> actions)
    {
        var targetIndex = sourceIndex.GetWithOffset(relativePosition);
        if (targetIndex == -1) return;
        var targetFigure = board[targetIndex];
        
        if (!targetFigure.IsWalkable())
        {
            return;
        }

        actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
    }
}