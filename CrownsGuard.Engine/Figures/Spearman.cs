using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Spearman
{
    private static readonly short[] BlackMovePositions =
    [
        PositionConstants.L1, PositionConstants.R1, PositionConstants.U1
    ];

    private static readonly short[] WhiteMovePositions =
    [
        PositionConstants.L1, PositionConstants.R1, PositionConstants.D1
    ];

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        var movePositions = sourceFigure.IsBlack() ? BlackMovePositions : WhiteMovePositions;
        foreach (var relative in movePositions)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
            }
        }

        foreach (var relative in PositionConstants.BishopDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (sourceFigure.CanAttack(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.MeleeAttack, sourceIndex, targetIndex, sourceFigure));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.PossibleMeleeAttack, sourceIndex, targetIndex, sourceFigure));
            }
        }
    }
}