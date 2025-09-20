using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.FigureDefinitions.Figures;

public static class Catapult
{
    private static readonly short[] BlackAttackPositions =
    [
        +unchecked((byte)-1)+2*PositionsGroups.YOffset, 
        unchecked((byte)+1)+2*PositionsGroups.YOffset,
        
        +unchecked((byte)-2)+3*PositionsGroups.YOffset, 
        unchecked((byte)+0)+3*PositionsGroups.YOffset, 
        unchecked((byte)+2)+3*PositionsGroups.YOffset,
    ];

    private static readonly short[] WhiteAttackPositions =
    [
        unchecked((byte)-1)-2*PositionsGroups.YOffset, 
        unchecked((byte)+1)-2*PositionsGroups.YOffset,
        
        unchecked((byte)-2)-3*PositionsGroups.YOffset, 
        unchecked((byte)+0)-3*PositionsGroups.YOffset, 
        unchecked((byte)+2)-3*PositionsGroups.YOffset,
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