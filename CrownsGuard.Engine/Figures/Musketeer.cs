using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Musketeer
{
    private static readonly short[] BlackAttackDirections =
    [
        PositionConstants.U1L1, PositionConstants.U1, PositionConstants.U1R1
    ];

    private static readonly short[] WhiteAttackDirections =
    [
        PositionConstants.D1L1, PositionConstants.D1, PositionConstants.D1R1
    ];

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionConstants.QueenDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (sourceFigure.IsEnemyTo(targetFigure))
            {
                return;
            }
        }

        var attackDirections = sourceFigure.IsBlack() ? BlackAttackDirections : WhiteAttackDirections;
        foreach (var relative in attackDirections)
        {
            var targetIndex = sourceIndex;
            for (var i = 1; i <= 3; i++)
            {
                targetIndex = targetIndex.GetWithOffset(relative);
                if (targetIndex == -1) break;
                var targetFigure = board[targetIndex];

                if (sourceFigure.CanAttack(targetFigure))
                {
                    actions.Push(new FigureAction(FigureActionType.RangedAttack, sourceIndex, targetIndex, sourceFigure));
                    break;
                }
                else if (targetFigure.IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleRangedAttack, sourceIndex, targetIndex, sourceFigure));
                }
                else
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleRangedAttack, sourceIndex, targetIndex, sourceFigure));
                    break;
                }
            }
        }
    }
}