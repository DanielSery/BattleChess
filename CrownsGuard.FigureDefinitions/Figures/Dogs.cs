using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Dogs : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Dogs;

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
            }
        }
        
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            for (var targetIndex = sourceIndex.GetWithOffset(relative); targetIndex != -1; targetIndex = targetIndex.GetWithOffset(relative))
            {
                var targetFigure = board[targetIndex];
                if (sourceFigure.CanAttack(targetFigure))
                {
                    actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourceIndex, targetIndex, sourceFigure));
                    break;
                }
                else if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourceIndex, targetIndex, sourceFigure));
                }
                else
                {
                    break;
                }
            }
        }
    }
}