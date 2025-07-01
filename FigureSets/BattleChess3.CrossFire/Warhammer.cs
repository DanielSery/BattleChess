using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Warhammer : ICrossFireFigureType
{
    int IFigureType.FigureId => 33;
    
    Position[] MovementPositions =>
    [
        new(-1, -1), new(-1, 0), new(-1, 1),
        new(0, -1), new(0, 1),
        new(1, -1), new(1, 0), new(1, 1)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovementPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }
    }

    void IFigureType.OnMoved(ITile unitTile, ITile targetTile, IBoard board)
    {
        var movement = targetTile.Position - unitTile.Position;
        switch (movement)
        {
            case { Y: 1, X: 1 }:
                targetTile.TryDestroyTile(board, new Position(1, 1));
                targetTile.TryDestroyTile(board, new Position(0, 1));
                targetTile.TryDestroyTile(board, new Position(1, 0));
                break;
            case { Y: -1, X: 1 }:
                targetTile.TryDestroyTile(board, new Position(1, -1));
                targetTile.TryDestroyTile(board, new Position(0, -1));
                targetTile.TryDestroyTile(board, new Position(1, 0));
                break;
            case { Y: 1, X: -1 }:
                targetTile.TryDestroyTile(board, new Position(-1, 1));
                targetTile.TryDestroyTile(board, new Position(0, 1));
                targetTile.TryDestroyTile(board, new Position(-1, 0));
                break;
            case { Y: -1, X: -1 }:
                targetTile.TryDestroyTile(board, new Position(-1, -1));
                targetTile.TryDestroyTile(board, new Position(0, -1));
                targetTile.TryDestroyTile(board, new Position(-1, 0));
                break;
            case { Y: 0, X: 1 }:
                targetTile.TryDestroyTile(board, new Position(1, -1));
                targetTile.TryDestroyTile(board, new Position(1, 0));
                targetTile.TryDestroyTile(board, new Position(1, 1));
                break;
            case { Y: 0, X: -1 }:
                targetTile.TryDestroyTile(board, new Position(-1, -1));
                targetTile.TryDestroyTile(board, new Position(-1, 0));
                targetTile.TryDestroyTile(board, new Position(-1, 1));
                break;
            case { Y: 1, X: 0 }:
                targetTile.TryDestroyTile(board, new Position(-1, 1));
                targetTile.TryDestroyTile(board, new Position(0, 1));
                targetTile.TryDestroyTile(board, new Position(1, 1));
                break;
            case { Y: -1, X: 0 }:
                targetTile.TryDestroyTile(board, new Position(-1, -1));
                targetTile.TryDestroyTile(board, new Position(0, -1));
                targetTile.TryDestroyTile(board, new Position(1, -1));
                break;
        }
    }
}