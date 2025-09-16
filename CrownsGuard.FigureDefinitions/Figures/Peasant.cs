using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Peasant : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Peasant;

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        if (sourceFigure.IsBlack())
        {
            TryAddMoveAction(board, sourceIndex, sourceFigure, unchecked((byte)+0)+1*PositionsGroups.YOffset, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, unchecked((byte)+0)+1*PositionsGroups.YOffset, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, unchecked((byte)-1)+0*PositionsGroups.YOffset, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, unchecked((byte)+1)+0*PositionsGroups.YOffset, actions);
        }
        else
        {
            TryAddMoveAction(board, sourceIndex, sourceFigure, unchecked((byte)+0)-1*PositionsGroups.YOffset, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, unchecked((byte)+0)-1*PositionsGroups.YOffset, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, unchecked((byte)-1)+0*PositionsGroups.YOffset, actions);
            TryAddAttackAction(board, sourceIndex, sourceFigure, unchecked((byte)+1)+0*PositionsGroups.YOffset, actions);
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
            actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourceIndex, targetIndex, sourceFigure));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourceIndex, targetIndex, sourceFigure));
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