using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class LegionaryPike
{
    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        if (sourceFigure.IsWhite())
        {
            TryAddPikeAttackAction(board, sourceIndex, sourceFigure, PositionConstants.D2R1, actions);
            TryAddPikeAttackAction(board, sourceIndex, sourceFigure, PositionConstants.D2L1, actions);
            
            TryAddWhiteAttackAction(board, sourceIndex, sourceFigure, PositionConstants.D1R1, actions);
            TryAddWhiteAttackAction(board, sourceIndex, sourceFigure, PositionConstants.D1L1, actions);
            
            if (sourceIndex >> 3 == 6)
            {
                TryAddStartMoveActions(board, sourceIndex, sourceFigure, PositionConstants.D1, actions);
            }
            else
            {
                TryAddWhiteMoveAction(board, sourceIndex, sourceFigure, PositionConstants.D1, actions);
            }
        }
        else
        {
            TryAddPikeAttackAction(board, sourceIndex, sourceFigure, PositionConstants.U2R1, actions);
            TryAddPikeAttackAction(board, sourceIndex, sourceFigure, PositionConstants.U2L1, actions);
            
            TryAddBlackAttackAction(board, sourceIndex, sourceFigure, PositionConstants.U1R1, actions);
            TryAddBlackAttackAction(board, sourceIndex, sourceFigure, PositionConstants.U1L1, actions);
            
            if (sourceIndex >> 3 == 1)
            {
                TryAddStartMoveActions(board, sourceIndex, sourceFigure, PositionConstants.U1, actions);
            }
            else
            {
                TryAddBlackMoveAction(board, sourceIndex, sourceFigure, PositionConstants.U1, actions);
            }
        }
    }

    private static void TryAddPikeAttackAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, short relativePosition,
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

    private static void TryAddBlackAttackAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, short relativePosition,
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

        if (targetIndex >> 3 == 7)
        {
            actions.Push(new FigureAction(FigureActionType.ChangeToQueen, sourceIndex, targetIndex, sourceFigure));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.MeleeAttack, sourceIndex, targetIndex, sourceFigure));
    }

    private static void TryAddWhiteAttackAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, short relativePosition,
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

        if (targetIndex >> 3 == 0)
        {
            actions.Push(new FigureAction(FigureActionType.ChangeToQueen, sourceIndex, targetIndex, sourceFigure));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.MeleeAttack, sourceIndex, targetIndex, sourceFigure));
    }

    private static void TryAddStartMoveActions(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, short relativePosition,
        Stack<FigureAction> actions)
    {
        var move1Index = sourceIndex.GetWithOffset(relativePosition);
        if (move1Index == -1) return;
        var move1Figure = board[move1Index];
        if (!move1Figure.IsWalkable()) return;
        actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, move1Index, sourceFigure));
        
        var move2Index = move1Index.GetWithOffset(relativePosition);
        if (move2Index == -1) return;
        var move2Figure = board[move2Index];
        if (!move2Figure.IsWalkable()) return;
        actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, move2Index, sourceFigure));
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