using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Mage : ICrownsGuardFigureTypeInfo
{
    public int FigureValue => 16;
    public FigureId FigureId => FigureId.Mage;

    private static readonly Position[] MovementPositions =
    [
        new(-2, -2), new(-2, 0), new(-2, 2),
        new(0, -2), new(0, 2),
        new(2, -2), new(2, 0), new(2, 2)
    ];

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;

        foreach (var relative in MovementPositions)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;

            if (targetFigure.IsWalkable())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
            }
        }
        
        return actions.ToArrayPoolMemory(actionsCount);
    }

    public static void ExecuteAction(Span<Figure> board, ref FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        switch (action.FigureActionType)
        {
            case FigureActionType.Move:
                var movement = action.TargetPosition - action.SourcePosition;
                board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
                if (Math.Abs(movement.X) == Math.Abs(movement.Y))
                {
                    TryDestroyTile(board, action.SourcePosition, new Position(1, 0), onEvent);
                    TryDestroyTile(board, action.SourcePosition, new Position(-1, 0), onEvent);
                    TryDestroyTile(board, action.SourcePosition, new Position(0, 1), onEvent);
                    TryDestroyTile(board, action.SourcePosition, new Position(0, -1), onEvent);
                }
                else
                {
                    TryDestroyTile(board, action.SourcePosition, new Position(1, -1), onEvent);
                    TryDestroyTile(board, action.SourcePosition, new Position(-1, 1), onEvent);
                    TryDestroyTile(board, action.SourcePosition, new Position(1, 1), onEvent);
                    TryDestroyTile(board, action.SourcePosition, new Position(-1, -1), onEvent);
                }
                break;
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }

    private static void TryDestroyTile(Span<Figure> board, Position sourcePosition, Position relative,
        Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (!board.TryGetFigure(sourcePosition + relative, out _))
            return;

        board.KillWithoutMove(sourcePosition, sourcePosition + relative, onEvent);
    }
}