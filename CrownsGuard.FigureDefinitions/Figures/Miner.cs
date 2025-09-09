using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Miner : ICrownsGuardFigureType
{
    public int FigureValue => 3;

    private static readonly Position[] Directions =
    [
        new(0, -1), new(0, 1),
        new(-1, 0), new(1, 0)
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
        foreach (var targetTile in Directions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }
        
        foreach (var direction in Directions)
        {
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 7, board, unitTile))
            {
                if (unitTile.CanMoveTo(targetTile))
                    yield return new FigureAction(
                        FigureActionTypes.Move,
                        targetTile.AbsolutePosition,
                        () => MoveAction(direction, unitTile, targetTile, board));
                else
                    break;
            }
        }
    }

    private void MoveAction(Position direction, ITile unitTile, ITile targetTile, IBoard board)
    {
        unitTile.MoveToTile(targetTile, board);
        for (var position = unitTile.RelativePosition; position != targetTile.RelativePosition; position += direction)
        {
            if (!board.TryGetTile(position, out var createdTile) ||
                !createdTile.IsEmpty())
            {
                continue;
            }
            
            createdTile.CreateFigure(new FigureInfo(NeutralFigureOwner.Instance, CrownsGuardFigureGroup.Trench, false), board);
        }
    }
}