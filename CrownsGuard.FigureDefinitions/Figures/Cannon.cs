using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Cannon : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Cannon;

    private static readonly Position[] BlackAttackPositions =
    [
        new(0, 2), new(0, 3), new(0, 4),
    ];

    private static readonly Position[] WhiteAttackPositions =
    [
        new(0, 2), new(0, 3), new(0, 4),
    ];

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board)
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
        
        var actionsMemory = ArrayPoolHelper.Rent<FigureAction>(3);
        var actions = actionsMemory.Span;
        int actionsCount = 0;
        var attackPositions = sourceFigure.PlayerColor == PlayerColor.Black ? BlackAttackPositions : WhiteAttackPositions;
        foreach (Position attackPosition in attackPositions)
        {
            var targetPosition = sourcePosition + attackPosition;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }
            
            if (sourceFigure.IsEnemyTo(targetFigure))
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, targetPosition);
            }
            else
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.PossibleAttack, sourcePosition, targetPosition);
            }
        }
        
        return actionsMemory.WithCount(actionsCount);
    }

    public static int EvaluateAction(ReadOnlySpan<Figure> board, FigureAction action)
    {
        if (action.FigureActionType == FigureActionType.PossibleAttack)
        {
            return Constants.PossibleRangedAttackImpact;
        }
        else if (action.FigureActionType == FigureActionType.Attack)
        {
            var figureValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
            return Constants.RangedAttackCoeff * figureValue + 2 * Constants.PossibleRangedAttackImpact;
        }
        else
        {
            return 0;
        }
    }

    public static void ExecuteAction(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var sourceFigure = board[action.SourcePosition.GetIndex()];
        var attackPositions = sourceFigure.PlayerColor == PlayerColor.Black ? BlackAttackPositions : WhiteAttackPositions;
        if (action.FigureActionType == FigureActionType.Attack)
        {
            foreach (var attackPosition in attackPositions)
            {
                var targetPosition = action.SourcePosition + attackPosition;
                if (!board.TryGetFigure(targetPosition, out var targetFigure))
                {
                    continue;
                }

                if (!targetFigure.IsEmpty())
                {
                    board.KillWithoutMove(action.SourcePosition, targetPosition, onEvent);
                }
            }
        }
        else
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
    }
}