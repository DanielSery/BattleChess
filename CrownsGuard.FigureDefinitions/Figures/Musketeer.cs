using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Musketeer : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Musketeer;

    private static readonly Position[] BlackAttackDirections =
    [
        new(-1, 1), new(0, 1), new(1, 1)
    ];

    private static readonly Position[] WhiteAttackDirections =
    [
        new(-1, -1), new(0, -1), new(1, -1)
    ];

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (sourceFigure.IsEnemyTo(targetFigure))
            {
                return ArrayPoolMemory<FigureAction>.Empty;
            }
        }

        var actionsMemory = ArrayPoolHelper.Rent<FigureAction>(36);
        var actions = actionsMemory.Span;
        int actionsCount = 0;
        var attackDirections = sourceFigure.PlayerColor == PlayerColor.Black ? BlackAttackDirections : WhiteAttackDirections;

        foreach (var relative in attackDirections)
        {
            var targetPosition = sourcePosition;
            for (var i = 1; i <= 3; i++)
            {
                targetPosition += relative;
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
        
        return actionsMemory.WithCount(actionsCount);;
    }

    public static int EvaluateAction(Span<Figure> board, FigureAction action)
    {
        if (action.FigureActionType == FigureActionType.PossibleAttack)
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
        if (action.FigureActionType == FigureActionType.Attack)
            board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
        else
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
    }
}