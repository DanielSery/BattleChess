using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Bard : ICrownsGuardFigureTypeInfo
{
    public int FigureValue => 12;
    public FigureId FigureId => FigureId.Bard;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;
        
        foreach (var relative in PositionsGroups.BishopDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;

            if (targetFigure.IsWalkable())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
            }
        }
        
        foreach (var relative in PositionsGroups.KnightPositions)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;
            
            if (sourceFigure.IsEnemyTo(targetFigure))
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, targetPosition);
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
            case FigureActionType.Special:
                board.ChangeOwner(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
}