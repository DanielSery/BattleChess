using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Ninja : ICrossFireFigureType
{
    int IFigureType.FigureId => 1;
    
    private readonly Position[] _attackPositions =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in _attackPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }

        if (unitTile.TryCreateMoveAction(board, new Position(-1, 1), out var move1Action))
            yield return move1Action;

        if (unitTile.TryCreateMoveAction(board, new Position(1, 1), out var move2Action))
            yield return move2Action;

        if (board.TryGetTile(unitTile.Position + new Position(0, 1), out var tileBefore) &&
            !tileBefore.IsEmpty() &&
            unitTile.TryCreateMoveAction(board, new Position(0, 2), out var move3Action))
        {
            yield return move3Action;
        }
    }
}