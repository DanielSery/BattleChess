using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Builder : ICrownsGuardFigureType
{
    public int FigureValue => 4;

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;
        
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out Figure targetFigure)) continue;

            if (targetFigure.IsWalkable())
            {
                actions[actionsCount++] = new FigureAction(sourceFigure.FigureType, FigureActionType.Move, sourcePosition, targetPosition);
            }
        }
        
        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out Figure targetFigure)) continue;

            if (targetFigure.IsEmpty())
            {
                actions[actionsCount++] = new FigureAction(sourceFigure.FigureType, FigureActionType.Special, sourcePosition, targetPosition);
            }
            else if (targetFigure.FigureType == FigureType.Wall)
            {
                actions[actionsCount++] = new FigureAction(sourceFigure.FigureType, FigureActionType.Attack, sourcePosition, targetPosition);
            }
        }
        
        return actions.ToArrayPool(actionsCount);
    }

    public static void ExecuteAction(Figure[] board, ref FigureAction action, Action<BoardEvent, Figure[]> onEvent)
    {
        switch (action.FigureActionType)
        {
            case FigureActionType.Move:
                board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            case FigureActionType.Attack:
                board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            case FigureActionType.Special:
                board.CreateFigure(action.TargetPosition, new Figure(Player.Neutral, false, FigureType.Wall), onEvent);
                break;
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType} for figure {action.FigureType}");
        }
    }
}