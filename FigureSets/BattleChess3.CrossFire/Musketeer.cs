using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Musketeer : ICrossFireFigureType
{
    int IFigureType.FigureId => 2;
    
    private readonly Position[] _attackDirections =
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
            if (unitTile.IsOwnedByEnemy(neighbourTile))
                yield break;
        }
        
        foreach (var direction in _attackDirections)
        {
            for (var i = 1; i < 8; i++)
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