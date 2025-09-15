using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Elephant : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Elephant;

    private static readonly Position[] Directions =
    [
        new(0, 1), new(0, -1)
    ];

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in Directions)
        {
            var isAttack = false;
            var targetPosition = sourcePosition;
            for (var i = 1; i <= 3; i++)
            {
                targetPosition += relative;
                if (!board.TryGetFigure(targetPosition, out var targetFigure))
                {
                    continue;
                }

                if (!isAttack && targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition, sourceFigure, targetFigure));
                    actions.Push(new FigureAction(FigureActionType.PossibleMeeleePierceAttack, sourcePosition, targetPosition, sourceFigure, targetFigure));
                }
                else
                {
                    isAttack = true;
                    actions.Push(new FigureAction(FigureActionType.MeeleePierceAttack, sourcePosition, targetPosition, sourceFigure, targetFigure));
                }
            }
        }
    }
}