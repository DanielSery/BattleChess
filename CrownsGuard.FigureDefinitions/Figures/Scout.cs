using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Scout : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Scout;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            for (var targetPosition = sourcePosition + relative; board.TryGetFigure(targetPosition, out var targetFigure); targetPosition += relative)
            {
                if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition));
                }
                else
                {
                    break;
                }
            }
        }

        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (sourceFigure.CanAttack(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourcePosition, targetPosition));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourcePosition, targetPosition));
            }
        }
    }
}