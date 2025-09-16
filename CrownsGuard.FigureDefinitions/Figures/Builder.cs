using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Builder : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Builder;

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
            }
        }
        
        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (targetFigure.IsEmpty())
            {
                actions.Push(new FigureAction(FigureActionType.BuildWall, sourceIndex, targetIndex, sourceFigure));
            }
            else if (targetFigure.GetFigureType() == Figure.Wall)
            {
                actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourceIndex, targetIndex, sourceFigure));
            }
        }
    }
}