using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Builder : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Builder;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out Figure targetFigure))
            {
                continue;
            }

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition));
            }
        }
        
        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out Figure targetFigure))
            {
                continue;
            }

            if (targetFigure.IsEmpty())
            {
                actions.Push(new FigureAction(FigureActionType.Special, sourcePosition, targetPosition));
            }
            else if (targetFigure.FigureType == FigureId.Wall)
            {
                actions.Push(new FigureAction(FigureActionType.Attack, sourcePosition, targetPosition));
            }
        }
        
        return;
    }

    public static int EvaluateAction(FigureAction action)
    {
        if (action.FigureActionType == FigureActionType.Special)
        {
            return Constants.BuildImpact;
        }
        else if (action.FigureActionType == FigureActionType.Move)
        {
            return Constants.MoveImpact;
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
            board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
        else if (action.FigureActionType == FigureActionType.Special)
            board.CreateFigure(action.TargetPosition, new Figure(PlayerColor.Neutral, false, FigureId.Wall), onEvent);
        else
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
    }
}