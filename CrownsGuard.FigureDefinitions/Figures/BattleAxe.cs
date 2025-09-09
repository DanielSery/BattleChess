using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class BattleAxe : ICrownsGuardFigureType
{
    public int FigureValue => 6;

    public int FigureId => (int)CrownsGuardFigureIds.BattleAxeId;
    
    private static readonly Position[] MovePositions =
    [
        new(-1, 1), new(1, 1), new(-1, -1), new(1, -1)
    ];
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovePositions.GetRelativeTiles(board, unitTile))
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
        switch (movement)
        {
            case { Y: 1, X: 1 }:
                targetTile.TryDestroyTile(board, new Position(1, 1));
                targetTile.TryDestroyTile(board, new Position(0, 1));
                targetTile.TryDestroyTile(board, new Position(1, 0));
                break;
            case { Y: -1, X: 1 }:
                targetTile.TryDestroyTile(board, new Position(1, -1));
                targetTile.TryDestroyTile(board, new Position(0, -1));
                targetTile.TryDestroyTile(board, new Position(1, 0));
                break;
            case { Y: 1, X: -1 }:
                targetTile.TryDestroyTile(board, new Position(-1, 1));
                targetTile.TryDestroyTile(board, new Position(0, 1));
                targetTile.TryDestroyTile(board, new Position(-1, 0));
                break;
            case { Y: -1, X: -1 }:
                targetTile.TryDestroyTile(board, new Position(-1, -1));
                targetTile.TryDestroyTile(board, new Position(0, -1));
                targetTile.TryDestroyTile(board, new Position(-1, 0));
                break;
        }
    }
}