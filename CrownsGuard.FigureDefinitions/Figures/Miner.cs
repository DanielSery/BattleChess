using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Miner : ICrownsGuardFigureType
{
    public int FigureValue => 3;
    public FigureId FigureId => FigureId.Miner;

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;

        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;

            if (targetFigure.IsWalkable())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
            }
        }

        foreach (var relative in PositionsGroups.RookDirections)
        {
            for (int i = 1; i < 7; i++)
            {
                var targetPosition = sourcePosition + relative * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;

                if (targetFigure.IsWalkable())
                    actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
                else
                    break;
            }
        }
        
        return actions.ToArrayPool(actionsCount);
    }

    public static void ExecuteAction(Figure[] board, ref FigureAction action, Action<BoardEvent, Figure[]> onEvent)
    {
        board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
        var difference = action.TargetPosition - action.SourcePosition;
        var direction = new Position((short) Math.Sign(difference.X), (short) Math.Sign(difference.Y));
        for (var position = action.SourcePosition; position != action.TargetPosition; position += direction)
        {
            if (!board.TryGetFigure(position, out var targetFigure)) continue;

            if (targetFigure.IsEmpty())
            {
                board.CreateFigure(position, new Figure(Player.Neutral, false, FigureId.Trench), onEvent);
            }
        }
    }
}