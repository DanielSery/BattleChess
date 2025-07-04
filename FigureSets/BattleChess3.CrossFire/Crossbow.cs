using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Crossbow : ICrossFireFigureType
{
    int IFigureType.FigureId => 35;
    
    protected Position[] AttackDirections =>
    [
        new(-1, 0), new(0, -1), new(0, 1), new(1, 0)
    ];

    protected Position[] MoveDirections =>
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];
    
    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var direction in MoveDirections)
        {
            for (var i = 1; i <= 2; i++)
            {
                if (!board.TryGetRelativeTile(unitTile, direction * i, out var targetTile))
                    continue;
            
                if (unitTile.CanMoveTo(targetTile))
                    yield return unitTile.CreateMoveAction(targetTile, board);
                else
                    break;
            }
        }
        
        foreach (var neighbourTile in ICrossFireFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.IsOwnedByEnemy(neighbourTile))
                yield break;
        }
        
        foreach (var direction in AttackDirections)
        {
            for (var i = 1; i <= 3; i++)
            {
                if (!board.TryGetRelativeTile(unitTile, direction * i, out var targetTile))
                    break;
                
                if (unitTile.CanAttack(targetTile))
                    yield return unitTile.CreateKillWithoutMove(targetTile, board);
                
                if (!unitTile.CanMoveTo(targetTile))
                    break;
            }
        }
    }
}