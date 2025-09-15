using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class CamelRider : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.CamelRider;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            for (var targetPosition = sourcePosition + relative; board.TryGetFigure(targetPosition, out var targetFigure); targetPosition += relative)
            {
                if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition, sourceFigure, targetFigure));
                }
                else
                {
                    break;
                }
            }
        }
        
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            for (var targetPosition = sourcePosition + relative; board.TryGetFigure(targetPosition, out var targetFigure); targetPosition += relative)
            {
                if (sourceFigure.CanAttack(targetFigure))
                {
                    actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourcePosition, targetPosition, sourceFigure, targetFigure));
                    break;
                }
                else if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourcePosition, targetPosition, sourceFigure, targetFigure));
                }
                else
                {
                    break;
                }
            }
        }
    }
}