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
        
        Span<FigureAction> actions = stackalloc FigureAction[3];
        int actionsCount = 0;
        var attackPositions = sourceFigure.PlayerColor == PlayerColor.Black ? BlackAttackPositions : WhiteAttackPositions;
        foreach (Position attackPosition in attackPositions)
        {
            if (!board.TryGetFigure(sourcePosition + attackPosition, out var attack1Figure) &&
                sourceFigure.IsEnemyTo(attack1Figure))
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, sourcePosition + new Position(0, 2));
            }
        }
        
        return actions.ToArrayPoolMemory(actionsCount);
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