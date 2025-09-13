using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Ninja : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Ninja;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        var direction = sourceFigure.PlayerColor == PlayerColor.Black ? 1 : -1;
        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (sourceFigure.CanAttack(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourcePosition, targetPosition));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourcePosition, targetPosition));
            }
        }

        if (TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(-1, (sbyte)(1 * direction)), out var action))
        {
            actions.Push(action);
        }

        if (TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(1, (sbyte)(1 * direction)), out action))
        {
            actions.Push(action);
        }

        if (TryGetMoveAction(board, sourcePosition, sourceFigure, new Position(0, (sbyte)(2 * direction)), out action) &&
            board.TryGetFigure(sourcePosition + new Position(0, (sbyte)(1 * direction)), out var jumpedOver) &&
            jumpedOver.IsAllyTo(sourceFigure))
        {
            actions.Push(action);
        }
    }

    private static bool TryGetMoveAction(ReadOnlySpan<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
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