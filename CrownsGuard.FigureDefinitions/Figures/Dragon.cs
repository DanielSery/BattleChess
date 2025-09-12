using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Dragon : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Dragon;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        var actionsMemory = ArrayPoolHelper.Rent<FigureAction>(36);
        var actions = actionsMemory.Span;
        int actionsCount = 0;
        
        foreach (var relative in PositionsGroups.RookDirections)
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
        }

        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (sourceFigure.IsEnemyTo(targetFigure))
            {
                return actionsMemory.WithCount(actionsCount);;
            }
        }
        
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetPosition = sourcePosition;
            for (var i = 1; i <= 2; i++)
            {
                targetPosition += relative;
                if (!board.TryGetFigure(targetPosition, out var targetFigure))
                {
                    break;
                }

                if (targetFigure.IsEmpty())
                {
                    actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, targetPosition);
                }
                else
                {
                    break;
                }
            }
        }
        
        return actionsMemory.WithCount(actionsCount);;
    }

    public static int EvaluateAction(Span<Figure> board, FigureAction action)
    {
        if (action.FigureActionType == FigureActionType.Special)
        {
            var move = action.TargetPosition - action.SourcePosition;
            if (move.X is <= 1 and >= -1 &&
                move.Y is <= 1 and >= -1)
            {
                return Constants.BuildImpact;
            }
            else if (move.X is <= 2 and >= -2 &&
                     move.Y is <= 2 and >= -2)
            {
                return Constants.BuildImpact * 2;
            }
        }
        else if (action.FigureActionType == FigureActionType.Move)
        {
            return Constants.MoveImpact;
        }
        
        return 0;
    }

    public static void ExecuteAction(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (action.FigureActionType == FigureActionType.Move)
        {
            board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
        }
        else if (action.FigureActionType == FigureActionType.Special)
        {
            var move = action.TargetPosition - action.SourcePosition;
            if (move.X is <= 1 and >= -1 &&
                move.Y is <= 1 and >= -1)
            {
                board.CreateFigure(action.TargetPosition, new Figure(PlayerColor.Neutral, false, FigureId.Fire),
                    onEvent);
            }
            else if (move.X is <= 2 and >= -2 &&
                     move.Y is <= 2 and >= -2)
            {
                var smallMove = new Position((sbyte)Math.Sign(move.X), (sbyte)Math.Sign(move.Y));

                board.CreateFigure(action.SourcePosition + smallMove,
                    new Figure(PlayerColor.Neutral, false, FigureId.Fire), onEvent);
                board.CreateFigure(action.TargetPosition, new Figure(PlayerColor.Neutral, false, FigureId.Fire),
                    onEvent);
            }
        }
        else
        {
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
}