using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Barbarian : ICrossFireFigureType
{
    public int FigureValue => 4;

    public int FigureId => CrossFireFigureIds.BarbarianId;
    
    private static readonly Position[] MovementPositions =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];
    
    private static readonly Position[] AttackDirections =
    [
        new(-1, -1), new(-1, 0), new(-1, 1),
        new(0, -1), new(0, 1),
        new(1, -1), new(1, 0), new(1, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovementPositions.GetRelativeTiles(board, unitTile))
        {
            if (targetTile.IsEmpty())
                yield return unitTile.CreateMoveAction(targetTile, board);
        }
        
        foreach (var direction in AttackDirections)
        {
            ITile? movedTile = null;
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 1, board, unitTile))
            {
                if (targetTile.IsEmpty()) 
                    continue;
                
                movedTile = targetTile;
                break;
            }
            
            if (movedTile is null)
            {
                continue;
            }
            
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 7, board, unitTile))
            {
                if (targetTile.RelativePosition == movedTile.RelativePosition)
                {
                }
                else if (targetTile.IsEmpty())
                {
                    yield return new FigureAction(
                        FigureActionTypes.Special,
                        targetTile.AbsolutePosition,
                        () => movedTile.MoveToTile(targetTile, board));
                }
            }
        }
    }
}