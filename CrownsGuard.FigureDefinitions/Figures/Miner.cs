using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Miner : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Miner;

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourceIndex, targetIndex, sourceFigure));
            }
        }

        foreach (var relative in PositionsGroups.RookDirections)
        {
            for (var targetIndex = sourceIndex.GetWithOffset(relative); targetIndex != -1; targetIndex = targetIndex.GetWithOffset(relative))
            {
                var targetFigure = board[targetIndex];
                if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.MinerMove, sourceIndex, targetIndex, sourceFigure));
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
        var difference = Position.FromIndex(action.TargetIndex - action.SourceIndex);
        var relative = (short)(Math.Sign(difference.X) + Math.Sign(difference.Y) * PositionsGroups.YOffset);
        
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