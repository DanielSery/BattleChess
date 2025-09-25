using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Alchemist
{
    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];
            
            if (targetFigure.IsWalkable() || targetFigure.GetFigureType() == Figure.Explosives)
            {
                actions.Push(new FigureAction(FigureActionType.AlchemistMove, sourceIndex, targetIndex, sourceFigure));
            }
        }
    }

    public static void ExecuteMove(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board.MoveFigure(action.SourceIndex, action.TargetIndex, onEvent);
        CreateExplosive(action, board, onEvent);
    }

    private static void CreateExplosive(FigureAction action, Span<Figure> board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var positionDiff = PositionsHelper.GetRelative(action.SourceIndex, action.TargetIndex);
        var targetIndex = action.SourceIndex.GetWithOffset(positionDiff);
        if (targetIndex == -1) return;
        var targetFigure = board[targetIndex];

        if (targetFigure.IsEmpty())
        {
            var sourceFigure = board[action.SourceIndex];
            board.CreateFigure((byte)targetIndex, Figure.Explosives | sourceFigure.GetFigureColor(), onEvent);
        }
    }
}