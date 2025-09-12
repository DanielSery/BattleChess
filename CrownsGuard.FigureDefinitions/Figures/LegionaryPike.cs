using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class LegionaryPike : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.LegionaryPike;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        if (sourceFigure.PlayerColor == PlayerColor.White)
            return GetPossibleWhiteActions(sourcePosition, sourceFigure, board);
        else return GetPossibleBlackActions(sourcePosition, sourceFigure, board);
    }

    public static ArrayPoolMemory<FigureAction> GetPossibleBlackActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        var actionsMemory = ArrayPoolHelper.Rent<FigureAction>(36);
        var actions = actionsMemory.Span;
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
            return actionsMemory.WithCount(actionsCount);;
        }

        if (sourcePosition.Y == 1 &&
            TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, 2), out var moveAction2))
        {
            actions[actionsCount++] = moveAction2;
        }
        
        return actionsMemory.WithCount(actionsCount);;
    }

    public static ArrayPoolMemory<FigureAction> GetPossibleWhiteActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        var actionsMemory = ArrayPoolHelper.Rent<FigureAction>(36);
        var actions = actionsMemory.Span;
        int actionsCount = 0;

        if (TryGetPikeAttackAction(board, sourcePosition, sourceFigure, new Position(1, -2), out var pikeAttackAction1))
        {
            actions[actionsCount++] = pikeAttackAction1;
        }

        if (TryGetPikeAttackAction(board, sourcePosition, sourceFigure, new Position(-1, -2), out var pikeAttackAction2))
        {
            actions[actionsCount++] = pikeAttackAction2;
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(1, -1), out var attackAction1))
        {
            actions[actionsCount++] = attackAction1;
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(-1, -1), out var attackAction2))
        {
            actions[actionsCount++] = attackAction2;
        }

        if (TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, -1), out var moveAction1))
        {
            actions[actionsCount++] = moveAction1;
        }
        else
        {
            return actionsMemory.WithCount(actionsCount);;
        }

        if (sourcePosition.Y == 6 &&
            TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, -2), out var moveAction2))
        {
            actions[actionsCount++] = moveAction2;
        }
        
        return actionsMemory.WithCount(actionsCount);;
    }

    public static int EvaluateAction(Span<Figure> board, FigureAction action)
    {
        if (action.FigureActionType == FigureActionType.Move)
        {
            return Constants.MoveImpact;
        }
        else if (action is { FigureActionType: FigureActionType.PossibleAttack, TargetPosition.Y: 2 or -1 })
        {
            var figureValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
            return Constants.PossibleRangedAttackImpact * figureValue;
        }
        else if (action is { FigureActionType: FigureActionType.PossibleAttack })
        {
            var figureValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
            return Constants.PossibleMeeleeAttackImpact * figureValue;
        }
        else if (action is { FigureActionType: FigureActionType.Attack, TargetPosition.Y: 2 or -1 })
        {
            var figureValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
            return Constants.RangedAttackCoeff * figureValue;
        }
        else if (action is { FigureActionType: FigureActionType.Attack })
        {
            var figureValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
            return Constants.MeeleeAttackCoeff * figureValue;
        }
        else
        {
            return 0;
        }
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
                    board.ChangeFigureType(action.TargetPosition, action.TargetPosition, FigureId.Blade, onEvent);
            }
            else
            {
                board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
                if (board[action.TargetPosition.GetIndex()].FigureType == FigureId.LegionaryPike)
                    board.ChangeFigureType(action.TargetPosition, action.TargetPosition, FigureId.Blade, onEvent);
            }
        }
        else
        {
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }

    private static bool TryGetPikeAttackAction(Span<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        out FigureAction action)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure))
        {
            action = new FigureAction(FigureActionType.None, sourcePosition, attackPosition);
            return false;
        }
        
        if (!sourceFigure.CanAttack(targetFigure))
        {
            action = new FigureAction(FigureActionType.PossibleAttack, sourcePosition, attackPosition);
            return true;
        }

        action = new FigureAction(FigureActionType.Attack, sourcePosition, attackPosition);
        return true;
    }

    private static bool TryGetAttackAction(Span<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        out FigureAction action)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure))
        {
            action = new FigureAction(FigureActionType.None, sourcePosition, attackPosition);
            return false;
        }
        
        if (!sourceFigure.CanAttack(targetFigure))
        {
            action = new FigureAction(FigureActionType.PossibleAttack, sourcePosition, attackPosition);
            return true;
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