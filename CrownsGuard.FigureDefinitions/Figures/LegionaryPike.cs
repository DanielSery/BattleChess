using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class LegionaryPike : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.LegionaryPike;

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        if (sourceFigure.IsWhite())
        {
            TryAddPikeAttackAction(board, sourceIndex, sourceFigure, Position.P1M2, actions);
            TryAddPikeAttackAction(board, sourceIndex, sourceFigure, Position.M1M2, actions);
            
            TryAddWhiteAttackAction(board, sourceIndex, sourceFigure, Position.P1M1, actions);
            TryAddWhiteAttackAction(board, sourceIndex, sourceFigure, Position.M1M1, actions);
            
            TryAddWhiteMoveAction(board, sourceIndex, sourceFigure, Position.P0M1, actions);
        
            if (sourceIndex >> 3 == 6)
            {
                TryAddWhiteMoveAction(board, sourceIndex, sourceFigure, Position.P0M2, actions);
            }
        }
        else
        {
            TryAddPikeAttackAction(board, sourceIndex, sourceFigure, Position.P1P2, actions);
            TryAddPikeAttackAction(board, sourceIndex, sourceFigure, Position.M1P2, actions);
            
            TryAddWhiteAttackAction(board, sourceIndex, sourceFigure, Position.P1P1, actions);
            TryAddWhiteAttackAction(board, sourceIndex, sourceFigure, Position.M1P1, actions);
            
            TryAddWhiteMoveAction(board, sourceIndex, sourceFigure, Position.P0P1, actions);
        
            if (sourceIndex >> 3 == 1)
            {
                TryAddWhiteMoveAction(board, sourceIndex, sourceFigure, Position.P0P2, actions);
            }
        }
    }

    private static void TryAddPikeAttackAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, Position relativePosition,
        Stack<FigureAction> actions)
    {
        var targetIndex = sourceIndex.GetWithOffset(relativePosition);
        if (targetIndex == -1) return;
        var targetFigure = board[targetIndex];
        
        if (!sourceFigure.CanAttack(targetFigure))
        {
            actions.Push(new FigureAction(FigureActionType.PossibleRangedAttack, sourceIndex, targetIndex, sourceFigure));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.RangedAttack, sourceIndex, targetIndex, sourceFigure));
    }

    private static void TryAddBlackAttackAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, Position relativePosition,
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

    private static void TryAddWhiteAttackAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, Position relativePosition,
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

    private static void TryAddBlackMoveAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, Position relativePosition,
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

    private static void TryAddWhiteMoveAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, Position relativePosition,
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