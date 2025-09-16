using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Pikeman : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Pikeman;

    private static readonly short[] BlackAttackPositions =
    [
        +unchecked((byte)-1)+1*PositionsGroups.YOffset, unchecked((byte)+1)+1*PositionsGroups.YOffset
    ];

    private static readonly short[] WhiteAttackPositions =
    [
        +unchecked((byte)-1)+1*PositionsGroups.YOffset, unchecked((byte)+1)+1*PositionsGroups.YOffset
    ];

    private static readonly short[] MovePositions =
    [
        +unchecked((byte)-1)+0*PositionsGroups.YOffset, unchecked((byte)+1)+0*PositionsGroups.YOffset, unchecked((byte)+0)-1*PositionsGroups.YOffset, unchecked((byte)+0)+1*PositionsGroups.YOffset
    ];

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in MovePositions)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
            }
        }

        var attackPositions = sourceFigure.IsBlack() ? BlackAttackPositions : WhiteAttackPositions;
        foreach (var relative in attackPositions)
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
    }
}
