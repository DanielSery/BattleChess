using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Catapult : ICrossFireFigureType
{
    public int FigureValue => 12;

    public int FigureId => CrossFireFigureIds.CatapultId;
    
    private static readonly Position[] AttackPositions =
    [
        new (-1, 2), new (1, 2),
        new (-2, 3), new (0, 3), new (2, 3),
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var neighbourTile in ICrossFireFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.IsEnemyTo(neighbourTile))
                yield break;
        }
        
        foreach (var targetTile in AttackPositions.GetRelativeTiles(board, unitTile))
        {
            if (!targetTile.IsEmpty())
                yield return unitTile.CreateKillWithoutMove(targetTile, board);
        }
    }
}