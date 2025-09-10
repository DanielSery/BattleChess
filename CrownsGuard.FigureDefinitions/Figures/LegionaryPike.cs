using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class LegionaryPike : ICrownsGuardFigureType
{
    public int FigureValue => 4;
    public FigureId FigureId => FigureId.LegionaryPike;

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;

        if (TryGetPikeAttackAction(board, sourcePosition, sourceFigure, new Position(1, 2), out var pikeAttackAction1))
        {
            actions[actionsCount++] = pikeAttackAction1;
        }

        if (TryGetPikeAttackAction(board, sourcePosition, sourceFigure, new Position(-1, 2), out var pikeAttackAction2))
        {
            actions[actionsCount++] = pikeAttackAction2;
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(1, 1), out var attackAction1))
        {
            actions[actionsCount++] = attackAction1;
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(-1, 1), out var attackAction2))
        {
            actions[actionsCount++] = attackAction2;
        }

        if (TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, 1), out var moveAction1))
        {
            actions[actionsCount++] = moveAction1;
        }
        else
        {
            return actions.ToArrayPool(actionsCount);
        }

        if (sourcePosition.Y == 1 &&
            TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, 2), out var moveAction2))
        {
            actions[actionsCount++] = moveAction2;
        }
        
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
            case FigureActionType.Special:
                var targetFigure = board[action.TargetPosition.GetIndex()];
                if (targetFigure.IsWalkable())
                {
                    board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
                    if (board[action.TargetPosition.GetIndex()].FigureType == FigureId.LegionaryPike)
                        board.ChangeFigureType(action.SourcePosition, action.TargetPosition, FigureId.Blade, onEvent);
                }
                break;
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }

    private static bool TryGetPikeAttackAction(Figure[] board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
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

        if (attackPosition.Y == 7)
        {
            action = new FigureAction(FigureActionType.Special, sourcePosition, attackPosition);
            return true;
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

        if (attackPosition.Y == 7)
        {
            action = new FigureAction(FigureActionType.Special, sourcePosition, attackPosition);
            return true;
        }

        action = new FigureAction(FigureActionType.Move, sourcePosition, attackPosition);
        return true;
    }
}