using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Ranger : ICrossFireFigureType
{
    int IFigureType.FigureId => 13;
    
    protected Position[] AttackDirections =>
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];

    protected Position[] MovePositions =>
    [
        new(-1, 0), new(0, -1), new(0, 1), new(1, 0)
    ];
    
    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var direction in AttackDirections)
        {
            for (var i = 1; i <= 3; i++)
            {
                var position = unitTile.Position + direction * i;
                if (!board.TryGetTile(position, out var targetTile))
                    break;
                
                if (targetTile.IsOwnedByEnemy(unitTile))
                {
                    yield return unitTile.CreateKillWithoutMove(targetTile, board);
                }
                
                if (targetTile.IsEmpty())
                {
                    if (i <= 2)
                    {
                        yield return unitTile.CreateMoveAction(targetTile, board);
                    }
                }
                else
                {
                    break;
                }
            }
        }
        
        foreach (var movement in MovePositions)
        {
            var position = unitTile.Position + movement;
            if (!board.TryGetTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsEmpty())
            {
                yield return unitTile.CreateMoveAction(targetTile, board);
            }
        }
    }
}