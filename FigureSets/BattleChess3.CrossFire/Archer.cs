using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Archer : ICrossFireFigureType
{
    int IFigureType.FigureId => 27;
    
    private readonly Position[] _directions =
    [
        new(-1, 0), new(1, 0),
        new(0, -1), new(0, 1)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var direction in _directions)
        {
            for (var i = 1; i <= 2; i++)
            {
                if (!board.TryGetRelativeTile(unitTile, direction * i, out var targetTile))
                    break;

                if (unitTile.CanMoveTo(targetTile))
                    yield return unitTile.CreateMoveAction(targetTile, board);
            }
        }
        
        foreach (var neighbourTile in ICrossFireFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.IsOwnedByEnemy(neighbourTile))
                yield break;
        }
        
        foreach (var direction in _directions)
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