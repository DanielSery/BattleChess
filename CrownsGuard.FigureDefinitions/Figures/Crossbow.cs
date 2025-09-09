using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Crossbow : ICrownsGuardFigureType
{
    public int FigureValue => 10;

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[18];
        int actionsCount = 0;
        
        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;

            if (targetFigure.IsWalkable())
            {
                actions[actionsCount++] = new FigureAction(sourceFigure.FigureType, FigureActionType.Move, sourcePosition, targetPosition);
            }
        }

        var currentFigure = board[sourcePosition.GetIndex()];
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;

            if (currentFigure.IsEnemyTo(targetFigure))
            {
                return actions.ToArrayPool(actionsCount);
            }
        }
        
        foreach (var direction in PositionsGroups.BishopDirections)
        {
            for (var i = 1; i <= 3; i++)
            {
                var targetPosition = sourcePosition + direction * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure)) break;

                if (currentFigure.CanAttack(targetFigure))
                {
                    actions[actionsCount++] = new FigureAction(sourceFigure.FigureType, FigureActionType.Attack, sourcePosition, targetPosition);
                    break;
                }
                
                if (!targetFigure.IsEmpty())
                {
                    break;
                }
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
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType} for figure {action.FigureType}");
        }
    }
}