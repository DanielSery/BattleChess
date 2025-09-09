using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Warhammer : ICrownsGuardFigureType
{
    public int FigureValue => 6;

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
                actions[actionsCount++] = new FigureAction(sourceFigure.FigureType, FigureActionType.Move, sourcePosition, targetPosition);
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
            case { Y: 0, X: 1 }:
                if (board.TryGetFigure(action.TargetPosition + new Position(1, -1), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                if (board.TryGetFigure(action.TargetPosition + new Position(1, 0), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                if (board.TryGetFigure(action.TargetPosition + new Position(1, 1), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            case { Y: 0, X: -1 }:
                if (board.TryGetFigure(action.TargetPosition + new Position(-1, -1), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                if (board.TryGetFigure(action.TargetPosition + new Position(-1, 0), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                if (board.TryGetFigure(action.TargetPosition + new Position(-1, 1), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            case { Y: 1, X: 0 }:
                if (board.TryGetFigure(action.TargetPosition + new Position(-1, 1), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                if (board.TryGetFigure(action.TargetPosition + new Position(0, 1), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                if (board.TryGetFigure(action.TargetPosition + new Position(1, 1), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            case { Y: -1, X: 0 }:
                if (board.TryGetFigure(action.TargetPosition + new Position(-1, -1), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                if (board.TryGetFigure(action.TargetPosition + new Position(0, -1), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                if (board.TryGetFigure(action.TargetPosition + new Position(1, -1), out Figure _)) board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                break;
        }
    }
}