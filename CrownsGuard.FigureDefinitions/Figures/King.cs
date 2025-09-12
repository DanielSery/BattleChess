using CrownsGuard.Core;
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
        var actionsMemory = ArrayPoolHelper.Rent<FigureAction>(36);
        var actions = actionsMemory.Span;
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
            else
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.PossibleAttack, sourcePosition, targetPosition);
            }
        }

        if (sourcePosition.X != 4)
        {
            return actionsMemory.WithCount(actionsCount);;
        }

        if (sourceFigure.PlayerColor == PlayerColor.Black)
        {
            if (sourcePosition.Y != 0)
            {
                return actionsMemory.WithCount(actionsCount);;
            }

            if (sourceFigure.IsAllyTo(board[0]) &&
                board[1].IsEmpty() &&
                board[2].IsEmpty() &&
                board[3].IsEmpty())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, new Position(2, 0));
            }

            if (sourceFigure.IsAllyTo(board[7]) &&
                board[5].IsEmpty() &&
                board[6].IsEmpty())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, new Position(6, 0));
            }
        }

        if (sourceFigure.PlayerColor == PlayerColor.White)
        {
            if (sourcePosition.Y != 7)
            {
                return actionsMemory.WithCount(actionsCount);;
            }

            if (sourceFigure.IsAllyTo(board[new Position(0, 7).GetIndex()]) &&
                board[new Position(1, 7).GetIndex()].IsEmpty() &&
                board[new Position(2, 7).GetIndex()].IsEmpty() &&
                board[new Position(3, 7).GetIndex()].IsEmpty())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, new Position(2, 7));
            }

            if (sourceFigure.IsAllyTo(board[new Position(7, 7).GetIndex()]) &&
                board[new Position(5, 7).GetIndex()].IsEmpty() &&
                board[new Position(6, 7).GetIndex()].IsEmpty())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, new Position(6, 7));
            }
        }

        return actionsMemory.WithCount(actionsCount);;
    }

    public static int EvaluateAction(Span<Figure> board, FigureAction action)
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
        else if (action.FigureActionType == FigureActionType.Special)
        {
            return Constants.MoveImpact * 2;
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
        else if (action is { FigureActionType: FigureActionType.Special, TargetPosition.X: 2 })
        {
            board.MoveFigure(new Position(4, action.TargetPosition.Y), new Position(2, action.TargetPosition.Y), onEvent);
            board.MoveFigure(new Position(0, action.TargetPosition.Y), new Position(3, action.TargetPosition.Y), onEvent);
        }
        else if (action is { FigureActionType: FigureActionType.Special, TargetPosition.X: 6 })
        {
            board.MoveFigure(new Position(4, action.TargetPosition.Y), new Position(6, action.TargetPosition.Y), onEvent);
            board.MoveFigure(new Position(7, action.TargetPosition.Y), new Position(5, action.TargetPosition.Y), onEvent);
        }
        else
        {
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
}