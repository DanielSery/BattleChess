using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Priest : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Priest;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board)
    {
        var actionsMemory = ArrayPoolHelper.Rent<FigureAction>(36);
        var actions = actionsMemory.Span;
        int actionsCount = 0;
        
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            for (var targetPosition = sourcePosition + relative; board.TryGetFigure(targetPosition, out var targetFigure); targetPosition += relative)
            {
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
            for (var targetPosition = sourcePosition + relative; board.TryGetFigure(targetPosition, out var targetFigure); targetPosition += relative)
            {
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

        return actionsMemory.WithCount(actionsCount);;
    }

    public static int EvaluateAction(ReadOnlySpan<Figure> board, FigureAction action)
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