
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Alchemist : ICrownsGuardFigureTypeInfo
{
    public int FigureValue => 4;
    public FigureId FigureId => FigureId.Alchemist;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
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

        return actions.ToArrayPoolMemory(actionsCount);
    }

    public static void ExecuteAction(Span<Figure> board, ref FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
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

    private static void CreateExplosive(Position sourcePosition, Position move, Span<Figure> board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetPosition = sourcePosition + move * 2;
        if (!board.TryGetFigure(targetPosition, out var targetFigure)) return;

        if (targetFigure.IsEmpty())
        {
            var sourceFigure = board[sourcePosition.GetIndex()];
            board.CreateFigure(targetPosition, new Figure(sourceFigure.PlayerColor, false, FigureId.Explosives), onEvent);
        }
    }
}