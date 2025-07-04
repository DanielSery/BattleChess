using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Chinese : ICrossFireFigureType
{
    int IFigureType.FigureId => 20;
    
    private readonly Position[] _moveDirections =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];

    private readonly Position[] _attackPositions =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, -1), new(-1, 1), new(-1, 2),
        new(1, -2), new(1, -1), new(1, 1), new(1, 2),
        new(2, -1), new(2, 1),
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var direction in _moveDirections)
        {
            for (var i = 1; i <= 2; i++)
            {
                if (!board.TryGetRelativeTile(unitTile, direction * i, out var targetTile))
                    break;

                if (unitTile.CanMoveTo(targetTile))
                    yield return unitTile.CreateMoveAction(targetTile, board);
            }
        }
        
        foreach (var targetTile in _attackPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }
    }
}