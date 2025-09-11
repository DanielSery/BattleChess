using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Archer : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Archer;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[18];
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
                return actions.ToArrayPoolMemory(actionsCount);
            }
        }
        
        foreach (var relative in PositionsGroups.RookDirections)
        {
            for (var i = 1; i <= 3; i++)
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
                else if (targetFigure.IsWalkable())
                {
                    actions[actionsCount++] = new FigureAction(FigureActionType.PossibleAttack, sourcePosition, targetPosition);
                }
                else
                {
                    break;
                }
            }
        }
        
        return actions.ToArrayPoolMemory(actionsCount);
    }

    public static int EvaluateAction(Span<Figure> board, FigureAction action)
    {
        if (action.FigureActionType == FigureActionType.Move)
        {
            return Constants.MoveImpact;
        }
        else if (action.FigureActionType == FigureActionType.PossibleAttack)
        {
            return Constants.PossibleRangedAttackImpact;
        }
        else if (action.FigureActionType == FigureActionType.Attack)
        {
            var figureValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
            return Constants.RangedAttackCoeff * figureValue;
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
        else
            throw new NotSupportedException($"Invalid action type {action.FigureActionType} for figure");
    }
}