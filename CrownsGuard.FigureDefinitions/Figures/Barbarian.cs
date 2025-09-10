using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Barbarian : ICrownsGuardFigureTypeInfo
{
    public int FigureValue => 4;
    public FigureId FigureId => FigureId.Barbarian;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;
        
        foreach (var relative in PositionsGroups.KnightPositions)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;

            if (targetFigure.IsWalkable())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
            }
        }
        
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var movedPosition = sourcePosition + relative;
            if (!board.TryGetFigure(movedPosition, out var movedFigure)) 
                continue;

            if (movedFigure.IsEmpty())
            {
                continue;
            }

            for (var i = 2; i < 8; i++)
            {
                var targetPosition = sourcePosition + relative * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure)) break;

                if (targetFigure.IsEmpty())
                {
                    actions[actionsCount++] = new FigureAction(FigureActionType.Special, movedPosition, targetPosition);
                }
                else
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
            case FigureActionType.Special:
                board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
}