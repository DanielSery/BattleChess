using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Spartan : ICrownsGuardFigureType
{
    public int FigureValue => 4;

    public int FigureId => (int)CrownsGuardFigureIds.SpartanId;
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in ICrownsGuardFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
            {
                yield return new FigureAction(
                    FigureActionTypes.Move,
                    targetTile.AbsolutePosition,
                    () => MoveAction(unitTile, targetTile, board));
            }
        }
    }

    private void MoveAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        unitTile.MoveToTile(targetTile, board);
        var movement = targetTile.RelativePosition - unitTile.RelativePosition;
        targetTile.TryDestroyTile(board, movement);
    }
}