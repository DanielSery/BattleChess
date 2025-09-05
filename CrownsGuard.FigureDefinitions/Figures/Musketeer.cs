using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Musketeer : ICrossFireFigureType
{
    public int FigureValue => 12;

    public int FigureId => CrossFireFigureIds.MusketeerId;
    
    private static readonly Position[] AttackDirections =
    [
        new(-1, 1), new(0, 1), new(1, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var neighbourTile in ICrossFireFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.IsEnemyTo(neighbourTile))
                yield break;
        }
        
        foreach (var direction in AttackDirections)
        {
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 3, board, unitTile))
            {
                if (unitTile.CanAttack(targetTile))
                    yield return unitTile.CreateKillWithoutMove(targetTile, board);

                if (!targetTile.IsEmpty())
                    break;
            }
        }
    }
}