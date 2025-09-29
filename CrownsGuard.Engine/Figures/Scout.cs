using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Scout
{
    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionConstants.QueenDirections)
        {
            for (var targetIndex = sourceIndex.GetWithOffset(relative); targetIndex != -1; targetIndex = targetIndex.GetWithOffset(relative))
            {
                var targetFigure = board[targetIndex];
                if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
                }
                else
                {
                    break;
                }
            }
        }

        foreach (var relative in PositionConstants.QueenDirections)
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