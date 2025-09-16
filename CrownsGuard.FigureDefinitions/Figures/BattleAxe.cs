using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class BattleAxe : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.BattleAxe;

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];
            
            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.BattleAxeMove, sourceIndex, targetIndex, sourceFigure));
            }
        }
    }

    public static void ExecuteMove(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board.MoveFigure(action.SourceIndex, action.TargetIndex, onEvent);
        var movement = action.TargetIndex - action.SourceIndex;
        if (movement == +1+1*Constants.BoardLength)
        {
            TryDestroyTile(board, action.TargetIndex, new Position(1, 1), onEvent);
            TryDestroyTile(board, action.TargetIndex, new Position(0, 1), onEvent);
            TryDestroyTile(board, action.TargetIndex, new Position(1, 0), onEvent);
        }
        else if (movement == +1-1*Constants.BoardLength)
        {
            TryDestroyTile(board, action.TargetIndex, new Position(1, -1), onEvent);
            TryDestroyTile(board, action.TargetIndex, new Position(0, -1), onEvent);
            TryDestroyTile(board, action.TargetIndex, new Position(1, 0), onEvent);
        }
        else if (movement == -1+1*Constants.BoardLength)
        {
            TryDestroyTile(board, action.TargetIndex, new Position(-1, 1), onEvent);
            TryDestroyTile(board, action.TargetIndex, new Position(0, 1), onEvent);
            TryDestroyTile(board, action.TargetIndex, new Position(-1, 0), onEvent);
        }
        else if (movement == -1-1*Constants.BoardLength)
        {
            TryDestroyTile(board, action.TargetIndex, new Position(-1, -1), onEvent);
            TryDestroyTile(board, action.TargetIndex, new Position(0, -1), onEvent);
            TryDestroyTile(board, action.TargetIndex, new Position(-1, 0), onEvent);
        }
    }
    
    private static void TryDestroyTile(Span<Figure> board, int sourceIndex, Position relative,
        Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetIndex = sourceIndex.GetWithOffset(relative);
        if (targetIndex == -1) return;

        board.KillWithoutMove((byte)sourceIndex, (byte)targetIndex, onEvent);
    }
}