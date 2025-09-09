using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Ranger : ICrownsGuardFigureType
{
    public int FigureValue => 10;

    public int FigureId => (int)CrownsGuardFigureIds.RangerId;
    
    private static readonly Position[] AttackDirections =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];

    private static readonly Position[] MovePositions =
    [
        new(-1, 0), new(0, -1), new(0, 1), new(1, 0)
    ];
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovePositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }
        
        foreach (var neighbourTile in ICrownsGuardFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
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