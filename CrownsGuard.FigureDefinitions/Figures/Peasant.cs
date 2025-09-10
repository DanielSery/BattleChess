using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Peasant : ICrownsGuardFigureType
{
    public int FigureValue => 2;
    public FigureId FigureId => FigureId.Peasant;

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;

        if (TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, 1), out var attackAction))
            actions[actionsCount++] = attackAction;

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(0, 1), out var move1Action))
            actions[actionsCount++] = move1Action;

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(-1, 0), out var move2Action))
            actions[actionsCount++] = move2Action;

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(1, 0), out var move3Action))
            actions[actionsCount++] = move3Action;
        
        return actions.ToArrayPool(actionsCount);
    }

    public static void ExecuteAction(Figure[] board, ref FigureAction action, Action<BoardEvent, Figure[]> onEvent)
    {
        switch (action.FigureActionType)
        {
            case FigureActionType.Move:
                board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            case FigureActionType.Attack:
                board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
    
    private static bool TryGetAttackAction(Figure[] board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        out FigureAction action)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure) ||
            !sourceFigure.CanAttack(targetFigure))
        {
            action = new FigureAction(FigureActionType.Move, Position.None, Position.None);
            return false;
        }

        action = new FigureAction(FigureActionType.Attack, sourcePosition, attackPosition);
        return true;
    }

    private static bool TryGetMoveAction(Figure[] board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        out FigureAction action)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure) ||
            !targetFigure.IsWalkable())
        {
            action = new FigureAction(FigureActionType.Move, Position.None, Position.None);
            return false;
        }

        action = new FigureAction(FigureActionType.Move, sourcePosition, attackPosition);
        return true;
    }
}