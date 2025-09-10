using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Scout : ICrownsGuardFigureTypeInfo
{
    public int FigureValue => 10;
    public FigureId FigureId => FigureId.Scout;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;

        foreach (var relative in PositionsGroups.QueenDirections)
        {
            for (var i = 1; i < 8; i++)
            {
                var targetPosition = sourcePosition + relative * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure))
                {
                    continue;
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

        foreach (var relative in PositionsGroups.QueenDirections)
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
        }

        return actions.ToArrayPoolMemory(actionsCount);
    }

    public static void ExecuteAction(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (action.FigureActionType == FigureActionType.Move)
            board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
        else if (action.FigureActionType == FigureActionType.Attack)
            board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
        else
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
    }
}