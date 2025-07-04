using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Musketeer : ICrossFireFigureType
{
    int IFigureType.FigureId => 2;
    
    private static readonly Position[] AttackDirections =
    [
        new(-1, -1), new(-1, 1),
        new(0, -1), new(0, 1),
        new(1, -1), new(1, 1)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        if (unitTile.TryCreateMoveAction(board, new Position(-1, 0), out var move1Action))
        {
            yield return move1Action;
        }
        
        if (unitTile.TryCreateMoveAction(board, new Position(1, 0), out var move2Action))
        {
            yield return move2Action;
        }
        
        foreach (var neighbourTile in ICrossFireFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.IsEnemyTo(neighbourTile))
                yield break;
        }
        
        foreach (var direction in AttackDirections)
        {
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 7, board, unitTile))
            {
                if (unitTile.CanAttack(targetTile))
                    yield return unitTile.CreateKillWithoutMove(targetTile, board);

                if (!targetTile.IsEmpty())
                    break;
            }
        }
    }
}