using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Trader : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Trader;

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
            else if (sourceFigure.CanAttack(targetFigure))
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, targetPosition);
            }
            else
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.PossibleAttack, sourcePosition, targetPosition);
            }
        }

        for (var i = 0; i < board.Length; i++)
        {
            var targetFigure = board[i];
            if (sourceFigure.IsAllyTo(targetFigure))
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, Position.FromIndex(i));
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
            board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
        else if (action.FigureActionType == FigureActionType.Special)
            board.SwapTiles(action.SourcePosition, action.TargetPosition, onEvent);
        else
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
    }
}