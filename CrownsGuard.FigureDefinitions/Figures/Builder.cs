using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Builder : ICrownsGuardFigureTypeInfo
{
    public int FigureValue => 4;
    public FigureId FigureId => FigureId.Builder;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;
        
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out Figure targetFigure))
            {
                continue;
            }

            if (targetFigure.IsWalkable())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
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
                actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, targetPosition);
            }
            else if (targetFigure.FigureType == FigureId.Wall)
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, targetPosition);
            }
        }
        
        return actions.ToArrayPoolMemory(actionsCount);
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