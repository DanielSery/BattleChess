using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Miner
{
    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionConstants.RookDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
            }
        }

        foreach (var relative in PositionConstants.RookDirections)
        {
            for (var targetIndex = sourceIndex.GetWithOffset(relative); targetIndex != -1; targetIndex = targetIndex.GetWithOffset(relative))
            {
                var targetFigure = board[targetIndex];
                if (targetFigure.IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.MinerMove, sourceIndex, targetIndex, sourceFigure));
                }
                else if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.MinerMove, sourceIndex, targetIndex, sourceFigure));
                    break;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public static void ExecuteMove(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board.MoveFigure(action.SourceIndex, action.TargetIndex, onEvent);
        var difference = PositionsHelper.GetRelative(action.SourceIndex, action.TargetIndex);
        var differenceX = PositionsHelper.GetRelativeX(difference);
        var differenceY = PositionsHelper.GetRelativeY(difference);
        var relative = (short)(Math.Sign(differenceX) + Math.Sign(differenceY) * PositionConstants.YOffset);
        
        for (var targetIndex = action.SourceIndex.GetWithOffset(relative); targetIndex != -1; targetIndex = targetIndex.GetWithOffset(relative))
        {
            var targetFigure = board[targetIndex];
            if (targetFigure.IsEmpty())
            {
                board.CreateFigure((byte)targetIndex, Figure.Trench, onEvent);
            }
            else
            {
                break;
            }
        }
    }
}