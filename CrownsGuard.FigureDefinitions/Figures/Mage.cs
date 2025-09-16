using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Mage : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Mage;

    private static readonly Position[] MovementPositions =
    [
        new(-2, -2), new(-2, 0), new(-2, 2),
        new(0, -2), new(0, 2),
        new(2, -2), new(2, 0), new(2, 2)
    ];

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in MovementPositions)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.MageMove, sourceIndex, targetIndex, sourceFigure));
            }
        }
    }

    public static void ExecuteMove(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (action.FigureActionType == FigureActionType.Move)
        {
            var movement = action.TargetIndex - action.SourceIndex;
            board.MoveFigure(action.SourceIndex, action.TargetIndex, onEvent);
            if (movement % 8 == movement / 8)
            {
                TryDestroyTile(board, action.SourceIndex, new Position(1, 0), onEvent);
                TryDestroyTile(board, action.SourceIndex, new Position(-1, 0), onEvent);
                TryDestroyTile(board, action.SourceIndex, new Position(0, 1), onEvent);
                TryDestroyTile(board, action.SourceIndex, new Position(0, -1), onEvent);
            }
            else
            {
                TryDestroyTile(board, action.SourceIndex, new Position(1, -1), onEvent);
                TryDestroyTile(board, action.SourceIndex, new Position(-1, 1), onEvent);
                TryDestroyTile(board, action.SourceIndex, new Position(1, 1), onEvent);
                TryDestroyTile(board, action.SourceIndex, new Position(-1, -1), onEvent);
            }
        }
        else
        {
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
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