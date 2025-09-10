using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class BattleAxe : ICrownsGuardFigureType
{
    public int FigureValue => 6;
    public FigureId FigureId => FigureId.BattleAxe;

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[4];
        int actionsCount = 0;

        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;
            
            if (targetFigure.IsWalkable())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
            }
        }
        
        return actions.ToArrayPool(actionsCount);
    }

    public static void ExecuteAction(Figure[] board, ref FigureAction action, Action<BoardEvent, Figure[]> onEvent)
    {
        board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
        var movement = action.TargetPosition - action.SourcePosition;
        switch (movement)
        {
            case { Y: 1, X: 1 }:
                TryDestroyTile(board, action.TargetPosition, new Position(1, 1), onEvent);
                TryDestroyTile(board, action.TargetPosition, new Position(0, 1), onEvent);
                TryDestroyTile(board, action.TargetPosition, new Position(1, 0), onEvent);
                break;
            case { Y: -1, X: 1 }:
                TryDestroyTile(board, action.TargetPosition, new Position(1, -1), onEvent);
                TryDestroyTile(board, action.TargetPosition, new Position(0, -1), onEvent);
                TryDestroyTile(board, action.TargetPosition, new Position(1, 0), onEvent);
                break;
            case { Y: 1, X: -1 }:
                TryDestroyTile(board, action.TargetPosition, new Position(-1, 1), onEvent);
                TryDestroyTile(board, action.TargetPosition, new Position(0, 1), onEvent);
                TryDestroyTile(board, action.TargetPosition, new Position(-1, 0), onEvent);
                break;
            case { Y: -1, X: -1 }:
                TryDestroyTile(board, action.TargetPosition, new Position(-1, -1), onEvent);
                TryDestroyTile(board, action.TargetPosition, new Position(0, -1), onEvent);
                TryDestroyTile(board, action.TargetPosition, new Position(-1, 01), onEvent);
                break;
        }
    }

    private static void TryDestroyTile(Figure[] board, Position sourcePosition, Position relative,
        Action<BoardEvent, Figure[]> onEvent)
    {
        if (!board.TryGetFigure(sourcePosition + relative, out _))
            return;

        board.KillWithoutMove(sourcePosition, sourcePosition + relative, onEvent);
    }
}