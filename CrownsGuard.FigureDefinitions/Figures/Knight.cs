using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Knight : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Knight;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition, sourceFigure, targetFigure));
            }
        }

        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition;
            for (var i = 1; i <= 3; i++)
            {
                targetPosition += relative;
                if (!board.TryGetFigure(targetPosition, out var targetFigure))
                {
                    break;
                }

                if (sourceFigure.CanAttack(targetFigure))
                {
                    actions.Push(new FigureAction(FigureActionType.MeeleePierceAttack, sourcePosition, targetPosition, sourceFigure, targetFigure));
                    break;
                }
                else if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleMeeleePierceAttack, sourcePosition, targetPosition, sourceFigure, targetFigure));
                }
                else
                {
                    break;
                }
            }
        }
    }
}