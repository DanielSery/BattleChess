using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class LegionarySword : ICrownsGuardFigureTypeInfo
{
    public int FigureValue => 2;
    public FigureId FigureId => FigureId.LegionarySword;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;

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
            return actions.ToArrayPoolMemory(actionsCount);
        }

        if (sourcePosition.Y == 1 &&
            TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, 2), out var moveAction2))
        {
            actions[actionsCount++] = moveAction2;
        }

        return actions.ToArrayPoolMemory(actionsCount);
        
    }

    public static void ExecuteAction(Span<Figure> board, ref FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
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

        if (attackPosition.Y == 7)
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

        if (attackPosition.Y == 7)
        {
            action = new FigureAction(FigureActionType.Special, sourcePosition, attackPosition);
            return true;
        }

        action = new FigureAction(FigureActionType.Move, sourcePosition, attackPosition);
        return true;
    }
}