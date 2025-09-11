using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class King : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.King;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;

        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (targetFigure.IsWalkable())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
            }

            if (sourceFigure.CanAttack(targetFigure))
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, targetPosition);
            }
        }

        if (sourcePosition.X != 4 && 
            (sourcePosition.Y != 0 || sourceFigure.PlayerColor != PlayerColor.Black) && 
            (sourcePosition.Y != 7 || sourceFigure.PlayerColor != PlayerColor.White))
        {
            return actions.ToArrayPoolMemory(actionsCount);
        }

        if (sourceFigure.IsAllyTo(board[0]) &&
            board[1].IsEmpty() &&
            board[2].IsEmpty() &&
            board[3].IsEmpty())
        {
            actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, new Position(2, sourcePosition.Y));
        }

        if (sourceFigure.IsAllyTo(board[7]) &&
            board[5].IsEmpty() &&
            board[6].IsEmpty())
        {
            actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, new Position(6, sourcePosition.Y));
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
        else if (action is { FigureActionType: FigureActionType.Special, TargetPosition.X: 2 })
        {
            board.MoveFigure(new Position(4, 0), new Position(2, 0), onEvent);
            board.MoveFigure(new Position(0, 0), new Position(3, 0), onEvent);
        }
        else if (action is { FigureActionType: FigureActionType.Special, TargetPosition.X: 6 })
        {
            board.MoveFigure(new Position(4, 0), new Position(6, 0), onEvent);
            board.MoveFigure(new Position(7, 0), new Position(5, 0), onEvent);
        }
        else
        {
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
}