using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Archer : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 10;
    
    int IFigureType.FigureId => 27;
    
    private static readonly Position[] MovePositions =
    [
        new(-1, 0), new(1, 0),
        new(0, -1), new(0, 1)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovePositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }
        
        foreach (var neighbourTile in ICrossFireFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.IsEnemyTo(neighbourTile))
                yield break;
        }
        
        foreach (var direction in MovePositions)
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