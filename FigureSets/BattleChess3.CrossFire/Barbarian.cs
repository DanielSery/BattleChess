using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Barbarian : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 5;
    
    int IFigureType.FigureId => 8;
    
    private static readonly Position[] MovementPositions =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];
    
    private static readonly Position[] AttackDirections =
    [
        new(-1, -1), new(-1, 0), new(-1, 1),
        new(0, -1), new(0, 1),
        new(1, -1), new(1, 0), new(1, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovementPositions.GetRelativeTiles(board, unitTile))
        {
            if (targetTile.IsEmpty())
                yield return unitTile.CreateMoveAction(targetTile, board);
        }
        
        foreach (var direction in AttackDirections)
        {
            var movedTile = NoneTile.Instance;
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 1, board, unitTile))
            {
                if (targetTile.IsEmpty()) 
                    continue;
                
                movedTile = targetTile;
                break;
            }
            
            if (movedTile == NoneTile.Instance)
            {
                continue;
            }
            
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 7, board, unitTile))
            {
                if (targetTile.Position == movedTile.Position)
                {
                }
                else if (targetTile.IsEmpty())
                {
                    yield return new FigureAction(
                        FigureActionTypes.Special,
                        unitTile.AbsolutePosition,
                        targetTile.AbsolutePosition,
                        () => movedTile.MoveToTile(targetTile, board));
                }
                else if (targetTile != NoneTile.Instance)
                {
                    break;
                }
            }
        }
    }
}