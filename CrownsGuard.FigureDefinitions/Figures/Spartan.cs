using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Spartan : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Spartan;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition));
            }
        }
    }

    public static void ExecuteMove(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (action.FigureActionType == FigureActionType.Move)
        {
            board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
            var sourceFigure = board[action.SourcePosition.GetIndex()];
            var attackedPosition = action.TargetPosition + action.TargetPosition - action.SourcePosition;

            if (!board.TryGetFigure(attackedPosition, out var targetFigure))
            {
                return;
            }

            if (sourceFigure.CanAttack(targetFigure))
            {
                board.KillWithoutMove(action.TargetPosition, attackedPosition, onEvent);
            }
        }
        else
        {
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
}