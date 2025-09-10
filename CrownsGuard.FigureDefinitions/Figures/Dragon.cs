using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Dragon : ICrownsGuardFigureType
{
    public int FigureValue => 12;
    public FigureId FigureId => FigureId.Dragon;

    private static readonly Position[] FireDirections =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];

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

        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;

            if (sourceFigure.IsEnemyTo(targetFigure))
            {
                return actions.ToArrayPool(actionsCount);
            }
        }
        
        foreach (var direction in PositionsGroups.BishopDirections)
        {
            for (var i = 1; i <= 2; i++)
            {
                var targetPosition = sourcePosition + direction * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure)) break;

                if (targetFigure.IsEmpty())
                {
                    actions[actionsCount++] = new FigureAction(FigureActionType.Special, sourcePosition, targetPosition);
                }
                else
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
            case FigureActionType.Move:
                board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
                break;
            case FigureActionType.Special:
                var move = action.TargetPosition - action.SourcePosition;
                if (move.X is <= 1 and >= -1 &&
                    move.Y is <= 1 and >= -1)
                {
                    board.CreateFigure(action.TargetPosition, new Figure(Player.Neutral, false, FigureId.Fire), onEvent);
                }
                else if (move.X is <= 2 and >= -2 &&
                         move.Y is <= 2 and >= -2)
                {
                    var smallMove = new Position((short)Math.Sign(move.X), (short)Math.Sign(move.Y));
            
                    board.CreateFigure(action.SourcePosition + smallMove, new Figure(Player.Neutral, false, FigureId.Fire), onEvent);
                    board.CreateFigure(action.TargetPosition, new Figure(Player.Neutral, false, FigureId.Fire), onEvent);
                }
                break;
            default:
                throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
    }
}