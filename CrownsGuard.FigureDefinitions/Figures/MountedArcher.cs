using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class MountedArcher : ICrossFireFigureType
{
    public int FigureValue => 10;

    public int FigureId => CrossFireFigureIds.MountedArcherId;
    
    private static readonly Position[] MoveDirections =
    [
        new(-1, 0),
        new(1, 0),
        new(0, -1),
        new(0, 1)
    ];

    private static readonly Position[] AttackDirections =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var direction in AttackDirections)
        {
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 7, board, unitTile))
            {
                if (unitTile.CanAttack(targetTile))
                    yield return unitTile.CreateKillWithMove(targetTile, board);

                if (!unitTile.CanMoveTo(targetTile))
                    break;
            }
        }
        
        foreach (var direction in MoveDirections)
        {
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 7, board, unitTile))
            {
                if (unitTile.CanMoveTo(targetTile))
                    yield return unitTile.CreateMoveAction(targetTile, board);
                else
                    break;
            }
        }
    }
}