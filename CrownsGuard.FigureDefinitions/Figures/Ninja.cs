using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Ninja : ICrownsGuardFigureType
{
    public int FigureValue => 3;

    private static readonly Position[] AttackPositions =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
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
        foreach (var targetTile in AttackPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }

        if (unitTile.TryCreateMoveAction(board, new Position(-1, 1), out var action))
            yield return action;

        if (unitTile.TryCreateMoveAction(board, new Position(1, 1), out action))
            yield return action;

        if (unitTile.CanMoveTo(board, new Position(0, 1)) &&
            unitTile.TryCreateMoveAction(board, new Position(0, 2), out action))
        {
            yield return action;
        }
    }
}