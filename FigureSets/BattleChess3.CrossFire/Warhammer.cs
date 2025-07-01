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
        foreach (var movementPosition in MovementPositions)
        {
            var position = unitTile.Position + movementPosition;
            if (!board.TryGetTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsEmpty())
            {
                yield return unitTile.CreateMoveAction(targetTile, board);
            }
        }
    }

    void IFigureType.OnMoved(ITile unitTile, ITile targetTile, IBoard board)
    {
        var movement = targetTile.Position - unitTile.Position;
        switch (movement)
        {
            case { Y: 1, X: 1 }:
                TryDestroyTile(targetTile, board, new Position(1, 1));
                TryDestroyTile(targetTile, board, new Position(0, 1));
                TryDestroyTile(targetTile, board, new Position(1, 0));
                break;
            case { Y: -1, X: 1 }:
                TryDestroyTile(targetTile, board, new Position(1, -1));
                TryDestroyTile(targetTile, board, new Position(0, -1));
                TryDestroyTile(targetTile, board, new Position(1, 0));
                break;
            case { Y: 1, X: -1 }:
                TryDestroyTile(targetTile, board, new Position(-1, 1));
                TryDestroyTile(targetTile, board, new Position(0, 1));
                TryDestroyTile(targetTile, board, new Position(-1, 0));
                break;
            case { Y: -1, X: -1 }:
                TryDestroyTile(targetTile, board, new Position(-1, -1));
                TryDestroyTile(targetTile, board, new Position(0, -1));
                TryDestroyTile(targetTile, board, new Position(-1, 0));
                break;
            case { Y: 0, X: 1 }:
                TryDestroyTile(targetTile, board, new Position(1, -1));
                TryDestroyTile(targetTile, board, new Position(1, 0));
                TryDestroyTile(targetTile, board, new Position(1, 1));
                break;
            case { Y: 0, X: -1 }:
                TryDestroyTile(targetTile, board, new Position(-1, -1));
                TryDestroyTile(targetTile, board, new Position(-1, 0));
                TryDestroyTile(targetTile, board, new Position(-1, 1));
                break;
            case { Y: 1, X: 0 }:
                TryDestroyTile(targetTile, board, new Position(-1, 1));
                TryDestroyTile(targetTile, board, new Position(0, 1));
                TryDestroyTile(targetTile, board, new Position(1, 1));
                break;
            case { Y: -1, X: 0 }:
                TryDestroyTile(targetTile, board, new Position(-1, -1));
                TryDestroyTile(targetTile, board, new Position(0, -1));
                TryDestroyTile(targetTile, board, new Position(1, -1));
                break;
        }
    }

    private static void TryDestroyTile(ITile unitTile, IBoard board, Position positionDiff)
    {
        if (!board.TryGetTile(unitTile.Position + positionDiff, out var targetTile))
            return;

        if (!targetTile.IsEmpty())
        {
            unitTile.KillWithoutMove(targetTile, board);
        }
    }
}