using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Priest : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Priest;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;
        
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            for (var i = 1; i < 7; i++)
            {
                var targetPosition = sourcePosition + relative * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure))
                {
                    break;
                }

                if (targetFigure.IsWalkable())
                {
                    actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
                }
                else
                {
                    break;
                }
            }
        }

        foreach (var relative in PositionsGroups.BishopDirections)
        {
            for (var i = 1; i < 7; i++)
            {
                var targetPosition = sourcePosition + relative * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure))
                {
                    break;
                }

                if (sourceFigure.CanAttack(targetFigure))
                {
                    actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, targetPosition);
                    break;
                }

                if (!targetFigure.IsWalkable())
                {
                    break;
                }
            }
        }

        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                break;
            }

            if (sourceFigure.IsAllyTo(targetFigure) ||
                sourceFigure.IsEnemyTo(targetFigure))
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, targetPosition);
            }
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
        else if (action.FigureActionType == FigureActionType.Special)
        {
            board.ChangeOwner(action.SourcePosition, action.TargetPosition, onEvent);
            board.MakeUnitKing(action.SourcePosition, action.TargetPosition, onEvent);
        }
        else
        {
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
}