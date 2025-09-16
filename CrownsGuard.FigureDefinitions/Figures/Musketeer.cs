using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Musketeer : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Musketeer;

    private static readonly short[] BlackAttackDirections =
    [
        +unchecked((byte)-1)+1*PositionsGroups.YOffset, unchecked((byte)+0)+1*PositionsGroups.YOffset, unchecked((byte)+1)+1*PositionsGroups.YOffset
    ];

    private static readonly short[] WhiteAttackDirections =
    [
        unchecked((byte)-1)-1*PositionsGroups.YOffset, unchecked((byte)+0)-1*PositionsGroups.YOffset, unchecked((byte)+1)-1*PositionsGroups.YOffset
    ];

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
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
                else if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleRangedAttack, sourceIndex, targetIndex, sourceFigure));
                }
                else
                {
                    break;
                }
            }
        }
    }
}