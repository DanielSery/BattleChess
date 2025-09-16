using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Dragon : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Dragon;

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

        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (sourceFigure.IsEnemyTo(targetFigure))
            {
                return;
            }
        }
        
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetIndex = sourceIndex;
            for (var i = 1; i <= 2; i++)
            {
                targetIndex = targetIndex.GetWithOffset(relative);
                if (targetIndex == -1) break;
                var targetFigure = board[targetIndex];

                if (targetFigure.IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.BreatheFire, sourceIndex, targetIndex, sourceFigure));
                }
                else
                {
                    break;
                }
            }
        }
    }

    public static void ExecuteBreatheFire(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var move = PositionsHelper.GetRelative(action.SourceIndex, action.TargetIndex);
        var moveX = PositionsHelper.GetRelativeX(move);
        var moveY = PositionsHelper.GetRelativeY(move);
        
        if (moveX is <= 1 and >= -1 && moveY is <= 1 and >= -1)
        {
            board.CreateFigure(action.TargetIndex, Figure.Fire, onEvent);
        }
        else if (moveX is <= 2 and >= -2 &&
                 moveY is <= 2 and >= -2)
        {
            var smallMove = PositionsHelper.GetRelativePosition(Math.Sign(moveX), Math.Sign(moveY));
            var targetIndex = action.SourceIndex.GetWithOffset(smallMove);

            board.CreateFigure((byte)targetIndex, Figure.Fire, onEvent);
            board.CreateFigure(action.TargetIndex, Figure.Fire, onEvent);
        }
    }
}