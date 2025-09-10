using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Musketeer : ICrownsGuardFigureType
{
    public int FigureValue => 12;
    public FigureId FigureId => FigureId.Musketeer;

    private static readonly Position[] AttackDirections =
    [
        new(-1, 1), new(0, 1), new(1, 1)
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

        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;

        foreach (var direction in AttackDirections)
        {
            for (var i = 1; i <= 3; i++)
            {
                var targetPosition = sourcePosition + direction * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure)) break;

                if (sourceFigure.CanAttack(targetFigure))
                {
                    actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, targetPosition);
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
            case FigureActionType.Attack:
                board.KillWithoutMove(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
}