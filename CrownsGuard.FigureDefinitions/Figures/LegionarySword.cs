using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class LegionarySword : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.LegionarySword;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;
        var direction = sourceFigure.PlayerColor == PlayerColor.Black ? 1 : -1;

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(1, (sbyte)(1 * direction)), out var attackAction1))
        {
            actions[actionsCount++] = attackAction1;
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(-1, (sbyte)(1 * direction)), out var attackAction2))
        {
            actions[actionsCount++] = attackAction2;
        }

        if (TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, (sbyte)(1 * direction)), out var moveAction1))
        {
            actions[actionsCount++] = moveAction1;
        }
        else
        {
            return actions.ToArrayPoolMemory(actionsCount);
        }

        if (sourcePosition.Y is 1 or 6 &&
            TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, (sbyte)(2 * direction)), out var moveAction2))
        {
            actions[actionsCount++] = moveAction2;
        }

        return actions.ToArrayPoolMemory(actionsCount);
        
    }

    public static void ExecuteAction(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (action.FigureActionType == FigureActionType.Move)
        {
            board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
        }
        else if (action.FigureActionType == FigureActionType.Attack)
        {
            board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
        }
        else if (action.FigureActionType == FigureActionType.Special)
        {
            var targetFigure = board[action.TargetPosition.GetIndex()];
            if (targetFigure.IsWalkable())
            {
                board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
                if (board[action.TargetPosition.GetIndex()].FigureType == FigureId.LegionaryPike)
                    board.ChangeFigureType(action.SourcePosition, action.TargetPosition, FigureId.Blade, onEvent);
            }
        }
        else
        {
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }

    private static bool TryGetAttackAction(Span<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        out FigureAction action)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure) ||
            !sourceFigure.CanAttack(targetFigure))
        {
            action = new FigureAction(FigureActionType.Move, Position.None, Position.None);
            return false;
        }

        if (attackPosition.Y is 7 or 0)
        {
            action = new FigureAction(FigureActionType.Special, sourcePosition, attackPosition);
            return true;
        }

        action = new FigureAction(FigureActionType.Attack, sourcePosition, attackPosition);
        return true;
    }

    private static bool TryGetMoveAction(Span<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        out FigureAction action)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure) ||
            !targetFigure.IsWalkable())
        {
            action = new FigureAction(FigureActionType.Move, Position.None, Position.None);
            return false;
        }

        if (attackPosition.Y is 7 or 0)
        {
            action = new FigureAction(FigureActionType.Special, sourcePosition, attackPosition);
            return true;
        }

        action = new FigureAction(FigureActionType.Move, sourcePosition, attackPosition);
        return true;
    }
}