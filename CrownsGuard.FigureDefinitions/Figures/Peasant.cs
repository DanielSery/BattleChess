using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Peasant : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Peasant;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        var direction = sourceFigure.PlayerColor == PlayerColor.Black ? 1 : -1;
        if (TryGetMoveAction(board, sourcePosition, new Position(0, (sbyte)(1 * direction)), out var attackAction))
        {
            actions.Push(attackAction);
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(0, (sbyte)(1 * direction)), out var move1Action))
        {
            actions.Push(move1Action);
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(-1, 0), out var move2Action))
        {
            actions.Push(move2Action);
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(1, 0), out var move3Action))
        {
            actions.Push(move3Action);
        }
        
        return;
    }

    public static int EvaluateAction(ReadOnlySpan<Figure> board, FigureAction action)
    {
        if (action.FigureActionType == FigureActionType.Move)
        {
            return Constants.MoveImpact;
        }
        else if (action.FigureActionType == FigureActionType.PossibleAttack)
        {
            return Constants.PossibleMeeleeAttackImpact;
        }
        else if (action.FigureActionType == FigureActionType.Attack)
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
            board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
        else if (action.FigureActionType == FigureActionType.Attack)
            board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
        else
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
    }
    
    private static bool TryGetAttackAction(ReadOnlySpan<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
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
            return false;
        }

        action = new FigureAction(FigureActionType.Attack, sourcePosition, attackPosition);
        return true;
    }

    private static bool TryGetMoveAction(ReadOnlySpan<Figure> board, Position sourcePosition, Position relativePosition,
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