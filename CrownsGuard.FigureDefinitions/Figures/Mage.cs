using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Mage : ICrownsGuardFigureType
{
    public int FigureValue => 16;

    private static readonly Position[] MovementPositions =
    [
        new(-2, -2), new(-2, 0), new(-2, 2),
        new(0, -2), new(0, 2),
        new(2, -2), new(2, 0), new(2, 2)
    ];

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;
        
        return actions.ToArrayPool(actionsCount);
    }

    public static void ExecuteAction(Figure[] board, ref FigureAction action, Action<BoardEvent, Figure[]> onEvent)
    {
        
    }

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovementPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
            {
                yield return new FigureAction(
                    FigureActionTypes.Move,
                    targetTile.AbsolutePosition,
                    () => MoveAction(unitTile, targetTile, board));
            }
        }
    }

    private void MoveAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        unitTile.MoveToTile(targetTile, board);
        var movement = targetTile.RelativePosition - unitTile.RelativePosition;
        if (Math.Abs(movement.X) == Math.Abs(movement.Y))
        {
            targetTile.TryDestroyTile(board, new Position(1, 0));
            targetTile.TryDestroyTile(board, new Position(-1, 0));
            targetTile.TryDestroyTile(board, new Position(0, 1));
            targetTile.TryDestroyTile(board, new Position(0, -1));
        }
        else
        {
            targetTile.TryDestroyTile(board, new Position(1, -1));
            targetTile.TryDestroyTile(board, new Position(-1, 1));
            targetTile.TryDestroyTile(board, new Position(1, 1));
            targetTile.TryDestroyTile(board, new Position(-1, -1));
        }
    }
}