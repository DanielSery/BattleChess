using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class LegionarySword : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.LegionarySword;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        if (sourceFigure.PlayerColor == PlayerColor.White)
            GetPossibleWhiteActions(sourcePosition, sourceFigure, board, actions);
        else GetPossibleBlackActions(sourcePosition, sourceFigure, board, actions);
    }

    public static void GetPossibleBlackActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board,
        Stack<FigureAction> actions)
    {
        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(1, 1), out var attackAction1))
        {
            actions.Push(attackAction1);
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(-1, 1), out var attackAction2))
        {
            actions.Push(attackAction2);
        }

        if (TryGetMoveAction(board, sourcePosition, new Position(0, 1), out var moveAction1))
        {
            actions.Push(moveAction1);
        }
        else
        {
            return;
        }

        if (sourcePosition.Y == 1 &&
            TryGetMoveAction(board, sourcePosition, new Position(0, 2), out var moveAction2))
        {
            actions.Push(moveAction2);
        }
    }

    public static void GetPossibleWhiteActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board,
        Stack<FigureAction> actions)
    {
        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(1, -1), out var attackAction1))
        {
            actions.Push(attackAction1);
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(-1, -1), out var attackAction2))
        {
            actions.Push(attackAction2);
        }

        if (TryGetMoveAction(board, sourcePosition, new Position(0, -1), out var moveAction1))
        {
            actions.Push(moveAction1);
        }
        else
        {
            return;
        }

        if (sourcePosition.Y == 6 &&
            TryGetMoveAction(board, sourcePosition, new Position(0, -2), out var moveAction2))
        {
            actions.Push(moveAction2);
        }
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
            action = new FigureAction(FigureActionType.PossibleMeeleeAttack, sourcePosition, attackPosition);
            return true;
        }

        if (attackPosition.Y is 7 or 0)
        {
            action = new FigureAction(FigureActionType.ChangeToQueen, sourcePosition, attackPosition);
            return true;
        }

        action = new FigureAction(FigureActionType.MeeleeAttack, sourcePosition, attackPosition);
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

        if (attackPosition.Y is 7 or 0)
        {
            action = new FigureAction(FigureActionType.ChangeToQueen, sourcePosition, attackPosition);
            return true;
        }

        action = new FigureAction(FigureActionType.Move, sourcePosition, attackPosition);
        return true;
    }
}