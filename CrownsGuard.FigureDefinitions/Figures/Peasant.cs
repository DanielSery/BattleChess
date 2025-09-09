using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Peasant : ICrownsGuardFigureType
{
    public int FigureValue => 2;

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
        if (unitTile.TryCreateMoveAction(board, new Position(0, 1), out var attackAction))
            yield return attackAction;

        if (unitTile.TryCreateKillWithMove(board, new Position(0, 1), out var move1Action))
            yield return move1Action;

        if (unitTile.TryCreateKillWithMove(board, new Position(-1, 0), out var move2Action))
            yield return move2Action;

        if (unitTile.TryCreateKillWithMove(board, new Position(1, 0), out var move3Action))
            yield return move3Action;
    }
}