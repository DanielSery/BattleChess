using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Peasant : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Peasant;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        var direction = sourceFigure.IsBlack() ? 1 : -1;
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
            return false;
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

        action = new FigureAction(FigureActionType.Move, sourcePosition, attackPosition);
        return true;
    }
}