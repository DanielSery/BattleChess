using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Trader : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Trader;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
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
            else if (sourceFigure.CanAttack(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourcePosition, targetPosition, sourceFigure, targetFigure));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourcePosition, targetPosition, sourceFigure, targetFigure));
            }
        }

        for (var i = 0; i < board.Length; i++)
        {
            var targetFigure = board[i];
            if (sourceFigure.IsAllyTo(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.SwapWithFigure, sourcePosition, Position.FromIndex(i), sourceFigure, targetFigure));
            }
        }
    }
}