using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Cannon : ICrownsGuardFigureType
{
    public int FigureValue => 12;
    public FigureId FigureId => FigureId.Cannon;

    private static readonly Position[] AttackPositions =
    [
        new(0, 2), new(0, 3), new(0, 4),
    ];

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;
            
            if (sourceFigure.IsEnemyTo(targetFigure))
                return [];
        }
        
        Span<FigureAction> actions = stackalloc FigureAction[3];
        int actionsCount = 0;
        
        if (!board.TryGetFigure(sourcePosition + new Position(0, 2), out var attack1Figure) &&
             sourceFigure.IsEnemyTo(attack1Figure))
            actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, sourcePosition + new Position(0, 2));
        
        if (!board.TryGetFigure(sourcePosition + new Position(0, 3), out var attack2Figure) &&
            sourceFigure.IsEnemyTo(attack2Figure))
            actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, sourcePosition + new Position(0, 3));
        
        if (!board.TryGetFigure(sourcePosition + new Position(0, 2), out var attack3Figure) &&
            sourceFigure.IsEnemyTo(attack3Figure))
            actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, sourcePosition + new Position(0, 4));
        
        return actions.ToArrayPool(actionsCount);
    }

    public static void ExecuteAction(Figure[] board, ref FigureAction action, Action<BoardEvent, Figure[]> onEvent)
    {
        switch (action.FigureActionType)
        {
            case FigureActionType.Attack:
                foreach (var attackPosition in AttackPositions)
                {
                    var targetPosition = action.SourcePosition + attackPosition;
                    if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;

                    if (!targetFigure.IsEmpty())
                    {
                        board.KillWithoutMove(action.SourcePosition, targetPosition, onEvent);
                    }
                }
                break;
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
}