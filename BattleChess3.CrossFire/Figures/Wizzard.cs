using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.CrossFireFigures.Utilities;

namespace BattleChess3.CrossFireFigures.Figures;

public class Wizzard : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 16;
    
    int IFigureType.FigureId => CrossFireFigureIds.WizzardId;
    
    private static readonly Position[] MovementPositions =
    [
        new(-2, -2), new(-2, 0), new(-2, 2),
        new(0, -2), new(0, 2),
        new(2, -2), new(2, 0), new(2, 2)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovementPositions.GetRelativeTiles(board, unitTile))
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
        if (Math.Abs(movement.X) != Math.Abs(movement.Y))
        {
            targetTile.TryDestroyTile(board, new Position(1, 0));
            targetTile.TryDestroyTile(board, new Position(-1, 0));
            targetTile.TryDestroyTile(board, new Position(0, 1));
            targetTile.TryDestroyTile(board, new Position(0, -1));
        }
        else
        {
            targetTile.TryDestroyTile(board, new Position(1, -1));
            targetTile.TryDestroyTile(board, new Position(-1, 1));
            targetTile.TryDestroyTile(board, new Position(1, 1));
            targetTile.TryDestroyTile(board, new Position(-1, -1));
        }
    }
}