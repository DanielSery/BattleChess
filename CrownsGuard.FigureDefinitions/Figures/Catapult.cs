using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Catapult : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Catapult;

    private static readonly Position[] BlackAttackPositions =
    [
        new (-1, 2), new (1, 2),
        new (-2, 3), new (0, 3), new (2, 3),
    ];

    private static readonly Position[] WhiteAttackPositions =
    [
        new (-1, -2), new (1, -2),
        new (-2, -3), new (0, -3), new (2, -3),
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
        
        var actionsMemory = ArrayPoolHelper.Rent<FigureAction>(5);
        var actions = actionsMemory.Span;
        int actionsCount = 0;
        var attackPositions = sourceFigure.PlayerColor == PlayerColor.Black ? BlackAttackPositions : WhiteAttackPositions;
        
        foreach (var relative in attackPositions)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (sourceFigure.CanAttack(targetFigure))
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, targetPosition);
            }
            else
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.PossibleAttack, sourcePosition, targetPosition);
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