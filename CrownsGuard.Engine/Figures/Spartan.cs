using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Spartan
{
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
        }
    }

    public static void ExecuteMove(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board.MoveFigure(action.SourceIndex, action.TargetIndex, onEvent);
        var diff = PositionsHelper.GetRelative(action.SourceIndex, action.TargetIndex);
        
        var targetIndex = action.TargetIndex.GetWithOffset(diff);
        if (targetIndex == -1) return;
        var targetFigure = board[targetIndex];

        if (action.SourceFigure.CanAttack(targetFigure))
        {
            board.KillWithoutMove(action.TargetIndex, (byte)targetIndex, onEvent);
        }
    }
}