using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Spearman : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Spearman;

    private static readonly short[] AttackPositions =
    [
        +unchecked((byte)-1)+1*PositionsGroups.YOffset, unchecked((byte)+1)+1*PositionsGroups.YOffset, unchecked((byte)-1)-1*PositionsGroups.YOffset, unchecked((byte)+1)-1*PositionsGroups.YOffset
    ];

    private static readonly short[] BlackMovePositions =
    [
        +unchecked((byte)-1)+0*PositionsGroups.YOffset, unchecked((byte)+1)+0*PositionsGroups.YOffset, unchecked((byte)+0)+1*PositionsGroups.YOffset
    ];

    private static readonly short[] WhiteMovePositions =
    [
        +unchecked((byte)-1)+0*PositionsGroups.YOffset, unchecked((byte)+1)+0*PositionsGroups.YOffset, unchecked((byte)+0)-1*PositionsGroups.YOffset
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

        foreach (var relative in AttackPositions)
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