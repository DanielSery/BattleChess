using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Barbarian : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Barbarian;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.KnightPositions)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition));
            }
        }
        
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var movedPosition = sourcePosition + relative;
            if (!board.TryGetFigure(movedPosition, out var movedFigure) ||
                movedFigure.IsEmpty())
            {
                continue;
            }

            for (var targetPosition = sourcePosition + relative + relative; board.TryGetFigure(targetPosition, out var targetFigure); targetPosition += relative)
            {
                if (targetFigure.IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.PushFigure, movedPosition, targetPosition));
                }
                else
                {
                    actions.Push(new FigureAction(FigureActionType.PossiblePushFigure, movedPosition, targetPosition));
                }
            }
        }
    }
}