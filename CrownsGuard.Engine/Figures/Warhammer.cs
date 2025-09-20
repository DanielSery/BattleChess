using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.FigureDefinitions.Figures;

public static class Warhammer
{
    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];
            
            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.WarhammerMove, sourceIndex, targetIndex, sourceFigure));
            }
        }
    }

    public static void ExecuteMove(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board.MoveFigure(action.SourceIndex, action.TargetIndex, onEvent);
        var movement = action.TargetIndex - action.SourceIndex;
        if (movement == +1+0*Constants.BoardLength)
        {
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)+1)-1*PositionsGroups.YOffset, onEvent);
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)+1)+0*PositionsGroups.YOffset, onEvent);
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)+1)+1*PositionsGroups.YOffset, onEvent);
        }
        else if (movement == -1+0*Constants.BoardLength)
        {
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)-1)-1*PositionsGroups.YOffset, onEvent);
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)-1)+0*PositionsGroups.YOffset, onEvent);
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)-1)+1*PositionsGroups.YOffset, onEvent);
        }
        else if (movement == +0+1*Constants.BoardLength)
        {
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)-1)+1*PositionsGroups.YOffset, onEvent);
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)+0)+1*PositionsGroups.YOffset, onEvent);
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)+1)+1*PositionsGroups.YOffset, onEvent);
        }
        else if (movement == +0-1*Constants.BoardLength)
        {
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)-1)+-1*PositionsGroups.YOffset, onEvent);
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)+0)+-1*PositionsGroups.YOffset, onEvent);
            TryDestroyTile(board, action.TargetIndex, unchecked((byte)+1)+-1*PositionsGroups.YOffset, onEvent);
        }
    }
    
    private static void TryDestroyTile(Span<Figure> board, int sourceIndex, short relative,
        Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetIndex = sourceIndex.GetWithOffset(relative);
        if (targetIndex == -1) return;

        board.KillWithoutMove((byte)sourceIndex, (byte)targetIndex, onEvent);
    }
}