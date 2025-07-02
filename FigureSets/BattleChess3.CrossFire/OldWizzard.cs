using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class OldWizzard : ICrossFireFigureType
{
    int IFigureType.FigureId => 3;
    
    private readonly Position[] _positions =
    [
        new(-2, -2), new(-2, 0), new(-2, 2),
        new(0, -2), new(0, 2),
        new(2, -2), new(2, 0), new(2, 2)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in _positions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }
    }

    void IFigureType.OnMoved(ITile unitTile, ITile targetTile, IBoard board)
    {
        targetTile.TryDestroyTile(board, new Position(-1, -1));
        targetTile.TryDestroyTile(board, new Position(-1, 0));
        targetTile.TryDestroyTile(board, new Position(-1, 1));
        targetTile.TryDestroyTile(board, new Position(0, -1));
        targetTile.TryDestroyTile(board, new Position(0, 1));
        targetTile.TryDestroyTile(board, new Position(1, -1));
        targetTile.TryDestroyTile(board, new Position(1, 0));
        targetTile.TryDestroyTile(board, new Position(1, 1));
    }
}