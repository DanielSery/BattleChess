using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Priest : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Priest;

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            for (var targetIndex = sourceIndex.GetWithOffset(relative); targetIndex != -1; targetIndex = targetIndex.GetWithOffset(relative))
            {
                var targetFigure = board[targetIndex];
                if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
                }
                else
                {
                    break;
                }
            }
        }

        foreach (var relative in PositionsGroups.BishopDirections)
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
                    actions.Push(new FigureAction(FigureActionType.MeeleeDefend, sourceIndex, targetIndex, sourceFigure));
                    break;
                }
            }
        }

        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (sourceFigure.IsAllyTo(targetFigure) ||
                sourceFigure.IsEnemyTo(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.MakeUnitKing, sourceIndex, targetIndex, sourceFigure));
            }
        }
    }

    public static void ExecuteMakeKing(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board.ConvertUnit(action.SourceIndex, action.TargetIndex, onEvent);
        board.MakeUnitKing(action.SourceIndex, action.TargetIndex, onEvent);
    }
}