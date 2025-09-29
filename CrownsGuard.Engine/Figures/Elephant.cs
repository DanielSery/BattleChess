using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Elephant
{
    private static readonly short[] Directions =
    [
        unchecked((byte)+0)+1*PositionsGroups.YOffset, 
        unchecked((byte)+0)-1*PositionsGroups.YOffset
    ];

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in Directions)
        {
            var isAttack = false;
            var targetIndex = sourceIndex;
            for (var i = 1; i <= 3; i++)
            {
                targetIndex = targetIndex.GetWithOffset(relative);
                if (targetIndex == -1) break;
                var targetFigure = board[targetIndex];

                if (!isAttack && targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
                    actions.Push(new FigureAction(FigureActionType.PossibleMeleePierceAttack, sourceIndex, targetIndex, sourceFigure));
                }
                else
                {
                    isAttack = true;
                    actions.Push(new FigureAction(FigureActionType.MeleePierceAttack, sourceIndex, targetIndex, sourceFigure));
                }
            }
        }
    }
}