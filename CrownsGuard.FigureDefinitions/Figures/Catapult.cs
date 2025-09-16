using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Catapult : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Catapult;

    private static readonly short[] BlackAttackPositions =
    [
        +-1+2*PositionsGroups.YOffset, 
        +1+2*PositionsGroups.YOffset,
        
        +-2+3*PositionsGroups.YOffset, 
        +0+3*PositionsGroups.YOffset, 
        +2+3*PositionsGroups.YOffset,
    ];

    private static readonly short[] WhiteAttackPositions =
    [
        +-1+-2*PositionsGroups.YOffset, 
        +1+-2*PositionsGroups.YOffset,
        
        +-2+-3*PositionsGroups.YOffset, 
        +0+-3*PositionsGroups.YOffset, 
        +2+-3*PositionsGroups.YOffset,
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
        
        var attackPositions = sourceFigure.IsBlack() ? BlackAttackPositions : WhiteAttackPositions;
        
        foreach (var relative in attackPositions)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (sourceFigure.CanAttack(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.RangedAttack, sourceIndex, targetIndex, sourceFigure));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.PossibleRangedAttack, sourceIndex, targetIndex, sourceFigure));
            }
        }
    }
}