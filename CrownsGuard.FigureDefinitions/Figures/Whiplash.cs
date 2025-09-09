using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Whiplash : ICrownsGuardFigureType
{
    public int FigureValue => 6;

    public int FigureId => (int)CrownsGuardFigureIds.WhiplashId;
    
    private static readonly Position[] AttackMovePositions =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in AttackMovePositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);

            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }
    }
}