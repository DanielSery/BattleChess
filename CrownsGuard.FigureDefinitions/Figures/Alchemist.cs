
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Alchemist : ICrownsGuardFigureType
{
    public int FigureValue => 4;
    public FigureId FigureId => FigureId.Alchemist;

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[8];
        var actionsCount = 0;
        
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out Figure targetFigure)) continue;
            
            if (targetFigure.IsWalkable() ||
                targetFigure.FigureType == FigureId.Explosives)
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
            }
        }

        return actions.ToArrayPool(actionsCount);
    }

    public static void ExecuteAction(Figure[] board, ref FigureAction action, Action<BoardEvent, Figure[]> onEvent)
    {
        switch (action.FigureActionType)
        {
            case FigureActionType.Move:
                board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
                CreateExplosive(action.SourcePosition, action.TargetPosition - action.SourcePosition, board, onEvent);
                break;
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }

    private static void CreateExplosive(Position sourcePosition, Position move, Figure[] board, Action<BoardEvent, Figure[]> onEvent)
    {
        var targetPosition = sourcePosition + move * 2;
        if (!board.TryGetFigure(targetPosition, out var targetFigure)) return;

        if (targetFigure.IsEmpty())
        {
            var sourceFigure = board[sourcePosition.GetIndex()];
            board.CreateFigure(targetPosition, new Figure(sourceFigure.Player, false, FigureId.Explosives), onEvent);
        }
    }
}