using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class LegionaryPike : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.LegionaryPike;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        if (sourceFigure.IsWhite())
            GetPossibleWhiteActions(sourcePosition, sourceFigure, board, actions);
        else GetPossibleBlackActions(sourcePosition, sourceFigure, board, actions);
    }

    public static void GetPossibleBlackActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board,
        Stack<FigureAction> actions)
    {
        if (TryGetPikeAttackAction(board, sourcePosition, sourceFigure, new Position(1, 2), out var pikeAttackAction1))
        {
            actions.Push(pikeAttackAction1);
        }

        if (TryGetPikeAttackAction(board, sourcePosition, sourceFigure, new Position(-1, 2), out var pikeAttackAction2))
        {
            actions.Push(pikeAttackAction2);
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(1, 1), out var attackAction1))
        {
            actions.Push(attackAction1);
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(-1, 1), out var attackAction2))
        {
            actions.Push(attackAction2);
        }

        if (TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, 1), out var moveAction1))
        {
            actions.Push(moveAction1);
        }
        else
        {
            return;
        }

        if (sourcePosition.Y == 1 &&
            TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, 2), out var moveAction2))
        {
            actions.Push(moveAction2);
        }
    }

    public static void GetPossibleWhiteActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board,
        Stack<FigureAction> actions)
    {
        if (TryGetPikeAttackAction(board, sourcePosition, sourceFigure, new Position(1, -2), out var pikeAttackAction1))
        {
            actions.Push(pikeAttackAction1);
        }

        if (TryGetPikeAttackAction(board, sourcePosition, sourceFigure, new Position(-1, -2), out var pikeAttackAction2))
        {
            actions.Push(pikeAttackAction2);
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(1, -1), out var attackAction1))
        {
            actions.Push(attackAction1);
        }

        if (TryGetAttackAction(board, sourcePosition, sourceFigure, new Position(-1, -1), out var attackAction2))
        {
            actions.Push(attackAction2);
        }

        if (TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, -1), out var moveAction1))
        {
            actions.Push(moveAction1);
        }
        else
        {
            return;
        }

        if (sourcePosition.Y == 6 &&
            TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, -2), out var moveAction2))
        {
            actions.Push(moveAction2);
        }
    }

    private static bool TryGetPikeAttackAction(ReadOnlySpan<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        out FigureAction action)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure))
        {
            action = new FigureAction(FigureActionType.None, sourcePosition, attackPosition, sourceFigure, Figure.Empty);
            return false;
        }
        
        if (!sourceFigure.CanAttack(targetFigure))
        {
            action = new FigureAction(FigureActionType.PossibleRangedAttack, sourcePosition, attackPosition, sourceFigure, targetFigure);
            return true;
        }

        action = new FigureAction(FigureActionType.RangedAttack, sourcePosition, attackPosition, sourceFigure, targetFigure);
        return true;
    }

    private static bool TryGetAttackAction(ReadOnlySpan<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        out FigureAction action)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure))
        {
            action = new FigureAction(FigureActionType.None, sourcePosition, attackPosition, sourceFigure, Figure.Empty);
            return false;
        }
        
        if (!sourceFigure.CanAttack(targetFigure))
        {
            action = new FigureAction(FigureActionType.PossibleMeeleeAttack, sourcePosition, attackPosition, sourceFigure, targetFigure);
            return true;
        }

        if (attackPosition.Y is 7 or 0)
        {
            action = new FigureAction(FigureActionType.ChangeToQueen, sourcePosition, attackPosition, sourceFigure, targetFigure);
            return true;
        }

        action = new FigureAction(FigureActionType.MeeleeAttack, sourcePosition, attackPosition, sourceFigure, targetFigure);
        return true;
    }

    private static bool TryGetMoveAction(ReadOnlySpan<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        out FigureAction action)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure) ||
            !targetFigure.IsWalkable())
        {
            action = new FigureAction(FigureActionType.Move, Position.None, Position.None, sourceFigure, Figure.Empty);
            return false;
        }

        if (attackPosition.Y is 7 or 0)
        {
            action = new FigureAction(FigureActionType.ChangeToQueen, sourcePosition, attackPosition, sourceFigure, Figure.Empty);
            return true;
        }

        action = new FigureAction(FigureActionType.Move, sourcePosition, attackPosition, sourceFigure, Figure.Empty);
        return true;
    }
}