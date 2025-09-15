using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class LegionarySword : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.LegionarySword;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        if (sourceFigure.IsWhite())
        {
            TryAddWhiteAttackAction(board, sourcePosition, sourceFigure, new Position(1, -1), actions);
            TryAddWhiteAttackAction(board, sourcePosition, sourceFigure, new Position(-1, -1), actions);
            TryAddWhiteMoveAction(board, sourcePosition, sourceFigure, new Position(0, -1), actions);
        
            if (sourcePosition.Y == 6)
            {
                TryAddWhiteMoveAction(board, sourcePosition, sourceFigure, new Position(0, -2), actions);
            }
        }
        else
        {
            TryAddBlackAttackAction(board, sourcePosition, sourceFigure, new Position(1, 1), actions);
            TryAddBlackAttackAction(board, sourcePosition, sourceFigure, new Position(-1, 1), actions);
            TryAddBlackMoveAction(board, sourcePosition, sourceFigure, new Position(0, 1), actions);
        
            if (sourcePosition.Y == 1)
            {
                TryAddBlackMoveAction(board, sourcePosition, sourceFigure, new Position(0, 2), actions);
            }
        }
    }

    private static void TryAddBlackAttackAction(ReadOnlySpan<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        Stack<FigureAction> actions)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure))
        {
            return;
        }
        
        if (!sourceFigure.CanAttack(targetFigure))
        {
            actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourcePosition, attackPosition, sourceFigure, targetFigure));
            return;
        }

        if (attackPosition.Y == 7)
        {
            actions.Push(new FigureAction(FigureActionType.ChangeToQueen, sourcePosition, attackPosition, sourceFigure, targetFigure));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourcePosition, attackPosition, sourceFigure, targetFigure));
    }

    private static void TryAddWhiteAttackAction(ReadOnlySpan<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        Stack<FigureAction> actions)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure))
        {
            return;
        }
        
        if (!sourceFigure.CanAttack(targetFigure))
        {
            actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourcePosition, attackPosition, sourceFigure, targetFigure));
            return;
        }

        if (attackPosition.Y == 0)
        {
            actions.Push(new FigureAction(FigureActionType.ChangeToQueen, sourcePosition, attackPosition, sourceFigure, targetFigure));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourcePosition, attackPosition, sourceFigure, targetFigure));
    }

    private static void TryAddBlackMoveAction(ReadOnlySpan<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        Stack<FigureAction> actions)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure) ||
            !targetFigure.IsWalkable())
        {
            return;
        }

        if (attackPosition.Y == 7)
        {
            actions.Push(new FigureAction(FigureActionType.ChangeToQueen, sourcePosition, attackPosition, sourceFigure, Figure.Empty));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, attackPosition, sourceFigure, Figure.Empty));
    }

    private static void TryAddWhiteMoveAction(ReadOnlySpan<Figure> board, Position sourcePosition, Figure sourceFigure, Position relativePosition,
        Stack<FigureAction> actions)
    {
        var attackPosition = sourcePosition + relativePosition;
        if (!board.TryGetFigure(attackPosition, out var targetFigure) ||
            !targetFigure.IsWalkable())
        {
            return;
        }

        if (attackPosition.Y == 0)
        {
            actions.Push(new FigureAction(FigureActionType.ChangeToQueen, sourcePosition, attackPosition, sourceFigure, Figure.Empty));
            return;
        }

        actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, attackPosition, sourceFigure, Figure.Empty));
    }
}