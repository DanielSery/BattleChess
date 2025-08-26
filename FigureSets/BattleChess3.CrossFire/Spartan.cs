using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;

namespace BattleChess3.CrossFireFigures;

public class Spartan : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 4;
    
    int IFigureType.FigureId => 12;
    
    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in ICrossFireFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
            {
                yield return new FigureAction(
                    FigureActionTypes.Move,
                    unitTile.AbsolutePosition,
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