using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class MountedKnight : ICrownsGuardFigureType
{
    public int FigureValue => 10;

    public int FigureId => (int)CrownsGuardFigureIds.MountedKnightId;
    
    private static readonly Position[] Directions =
    [
        new(0, -1), new(0, 1),
        new(-1, 0), new(1, 0)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var direction in Directions)
        {
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 7, board, unitTile))
            {
                if (unitTile.CanAttack(targetTile))
                    yield return unitTile.CreateKillWithMove(targetTile, board);
                
                if (unitTile.CanMoveTo(targetTile))
                    yield return unitTile.CreateMoveAction(targetTile, board);
                else
                    break;
            }
        }
    }
}