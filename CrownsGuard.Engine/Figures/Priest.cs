using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Priest
{
    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionConstants.BishopDirections)
        {
            for (var targetIndex = sourceIndex.GetWithOffset(relative); targetIndex != -1; targetIndex = targetIndex.GetWithOffset(relative))
            {
                var targetFigure = board[targetIndex];
                if (targetFigure.IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
                }
                else if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
                    break;
                }
                else
                {
                    break;
                }
            }
        }

        foreach (var relative in PositionConstants.BishopDirections)
        {
            for (var targetIndex = sourceIndex.GetWithOffset(relative); targetIndex != -1; targetIndex = targetIndex.GetWithOffset(relative))
            {
                var targetFigure = board[targetIndex];
                if (sourceFigure.CanAttack(targetFigure))
                {
                    actions.Push(new FigureAction(FigureActionType.MeleeAttack, sourceIndex, targetIndex, sourceFigure));
                    break;
                }
                else if (targetFigure.IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleMeleeAttack, sourceIndex, targetIndex, sourceFigure));
                }
                else
                {
                    actions.Push(new FigureAction(FigureActionType.MeleeDefend, sourceIndex, targetIndex, sourceFigure));
                    break;
                }
            }
        }

        foreach (var relative in PositionConstants.RookDirections)
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