using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Builder : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Builder;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out Figure targetFigure))
            {
                continue;
            }

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition));
            }
        }
        
        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out Figure targetFigure))
            {
                continue;
            }

            if (targetFigure.IsEmpty())
            {
                actions.Push(new FigureAction(FigureActionType.BuildWall, sourcePosition, targetPosition));
            }
            else if (targetFigure.FigureType == FigureId.Wall)
            {
                actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourcePosition, targetPosition));
            }
        }
    }
}