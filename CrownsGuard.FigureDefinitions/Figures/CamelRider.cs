using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class CamelRider : ICrownsGuardFigureTypeInfo
{
    public int FigureValue => 6;
    public FigureId FigureId => FigureId.CamelRider;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;
        
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            for (var i = 1; i < 7; i++)
            {
                var targetPosition = sourcePosition + relative * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure)) break;

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
            for (var i = 1; i < 7; i++)
            {
                var targetPosition = sourcePosition + relative * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure)) break;

                if (sourceFigure.CanAttack(targetFigure))
                {
                    actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, targetPosition);
                }
                else if (!targetFigure.IsWalkable())
                {
                    break;
                }
            }
        }
        
        return actions.ToArrayPoolMemory(actionsCount);
    }

    public static void ExecuteAction(Span<Figure> board, ref FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        switch (action.FigureActionType)
        {
            case FigureActionType.Move:
                board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            case FigureActionType.Attack:
                board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
}