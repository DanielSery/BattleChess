using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Trader : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Trader;

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
            else if (sourceFigure.CanAttack(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourceIndex, targetIndex, sourceFigure));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourceIndex, targetIndex, sourceFigure));
            }
        }

        for (var i = 0; i < board.Length; i++)
        {
            var targetFigure = board[i];
            if (sourceFigure.IsAllyTo(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.SwapWithFigure, sourceIndex, i, sourceFigure));
            }
        }
    }
}