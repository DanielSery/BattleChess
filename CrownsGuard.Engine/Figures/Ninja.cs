using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.FigureDefinitions.Figures;

public static class Ninja
{
    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (sourceFigure.CanAttack(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourceIndex, targetIndex, sourceFigure));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourceIndex, targetIndex, sourceFigure));
            }
        }

        if (sourceFigure.IsBlack())
        {
            TryAddMoveAction(board, sourceIndex, sourceFigure, +unchecked((byte)-1)+1*PositionsGroups.YOffset, actions);
            TryAddMoveAction(board, sourceIndex, sourceFigure, unchecked((byte)+1)+1*PositionsGroups.YOffset, actions);
            TryAddJumpMoveAction(board, sourceIndex, sourceFigure, unchecked((byte)+0)+1*PositionsGroups.YOffset, actions);
        }
        else
        {
            TryAddMoveAction(board, sourceIndex, sourceFigure, unchecked((byte)-1)-1*PositionsGroups.YOffset, actions);
            TryAddMoveAction(board, sourceIndex, sourceFigure, unchecked((byte)+1)-1*PositionsGroups.YOffset, actions);
            TryAddJumpMoveAction(board, sourceIndex, sourceFigure, unchecked((byte)+0)-1*PositionsGroups.YOffset, actions);
        }
    }

    private static void TryAddJumpMoveAction(ReadOnlySpan<Figure> board, int sourceIndex, Figure sourceFigure, short relativePosition,
        Stack<FigureAction> actions)
    {
        var figureIndex = sourceIndex.GetWithOffset(relativePosition);
        if (figureIndex == -1) return;
        var figure = board[figureIndex];

        if (!sourceFigure.IsAllyTo(figure))
        {
            return;
        }
        
        var targetIndex = figureIndex.GetWithOffset(relativePosition);
        if (targetIndex == -1) return;
        var targetFigure = board[targetIndex];
        
        if (!targetFigure.IsWalkable())
        {
            return;
        }

        actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
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