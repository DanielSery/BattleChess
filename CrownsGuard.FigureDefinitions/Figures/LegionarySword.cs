using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class LegionarySword : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.LegionarySword;

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        if (sourceFigure.IsWhite())
        {
            TryAddWhiteAttackAction(board, sourceIndex, sourceFigure, unchecked((byte)+1)-1*PositionsGroups.YOffset, actions);
            TryAddWhiteAttackAction(board, sourceIndex, sourceFigure, unchecked((byte)-1)-1*PositionsGroups.YOffset, actions);
            
            TryAddWhiteMoveAction(board, sourceIndex, sourceFigure, unchecked((byte)+0)-1*PositionsGroups.YOffset, actions);
        
            if (sourceIndex >> 3 == 6)
            {
                TryAddWhiteMoveAction(board, sourceIndex, sourceFigure, unchecked((byte)+0)-2*PositionsGroups.YOffset, actions);
            }
        }
        else
        {
            TryAddBlackAttackAction(board, sourceIndex, sourceFigure, unchecked((byte)+1)+1*PositionsGroups.YOffset, actions);
            TryAddBlackAttackAction(board, sourceIndex, sourceFigure, +unchecked((byte)-1)+1*PositionsGroups.YOffset, actions);
            
            TryAddBlackMoveAction(board, sourceIndex, sourceFigure, unchecked((byte)+0)+1*PositionsGroups.YOffset, actions);
        
            if (sourceIndex >> 3 == 1)
            {
                TryAddBlackMoveAction(board, sourceIndex, sourceFigure, unchecked((byte)+0)+2*PositionsGroups.YOffset, actions);
            }
        }
    }

    private static void TryAddBlackAttackAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, short relativePosition,
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

        if (targetIndex >> 3 == 7)
        {
            actions.Push(new FigureAction(FigureActionType.ChangeToQueen, sourceIndex, targetIndex, sourceFigure));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourceIndex, targetIndex, sourceFigure));
    }

    private static void TryAddWhiteAttackAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, short relativePosition,
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

        if (targetIndex >> 3 == 0)
        {
            actions.Push(new FigureAction(FigureActionType.ChangeToQueen, sourceIndex, targetIndex, sourceFigure));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourceIndex, targetIndex, sourceFigure));
    }

    private static void TryAddBlackMoveAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, short relativePosition,
        Stack<FigureAction> actions)
    {
        var targetIndex = sourceIndex.GetWithOffset(relativePosition);
        if (targetIndex == -1) return;
        var targetFigure = board[targetIndex];
        
        if (!targetFigure.IsWalkable())
        {
            return;
        }

        if (targetIndex >> 3 == 7)
        {
            actions.Push(new FigureAction(FigureActionType.ChangeToQueen, sourceIndex, targetIndex, sourceFigure));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
    }

    private static void TryAddWhiteMoveAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, short relativePosition,
        Stack<FigureAction> actions)
    {
        var targetIndex = sourceIndex.GetWithOffset(relativePosition);
        if (targetIndex == -1) return;
        var targetFigure = board[targetIndex];
        
        if (!targetFigure.IsWalkable())
        {
            return;
        }

        if (targetIndex >> 3 == 0)
        {
            actions.Push(new FigureAction(FigureActionType.ChangeToQueen, sourceIndex, targetIndex, sourceFigure));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
    }
}