using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.StarWarsFigures;

public class AnakinGrievus : IStarWarsFigureType, IFigureTypeWithDifferentMoves
{
    int[] IFigureTypeWithDifferentMoves.Actions { get; } =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 3, 0, 1, 0, 3, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 1, 3, 8, 3, 1, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 3, 0, 1, 0, 3, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    void IFigureType.OnMoved(ITile unitTile, ITile targetTile, ITile[] board)
    {
        var movement = targetTile.Position - unitTile.Position;
        if (Math.Abs(movement.X) == Math.Abs(movement.Y))
        {
            TryDestroyTile(targetTile, board, new Position(1, 0));
            TryDestroyTile(targetTile, board, new Position(-1, 0));
            TryDestroyTile(targetTile, board, new Position(0, 1));
            TryDestroyTile(targetTile, board, new Position(0, -1));
        }
        else
        {
            TryDestroyTile(targetTile, board, new Position(1, -1));
            TryDestroyTile(targetTile, board, new Position(-1, 1));
            TryDestroyTile(targetTile, board, new Position(1, 1));
            TryDestroyTile(targetTile, board, new Position(-1, -1));
        }
    }

    private static void TryDestroyTile(ITile unitTile, ITile[] board, Position positionDiff)
    {
        var targetPosition = unitTile.Position + positionDiff;
        if (!targetPosition.IsInBoard())
        {
            return;
        }

        var target = board[targetPosition];
        if (!target.IsEmpty() &&
            !unitTile.Figure.Owner.Equals(target.Figure.Owner))
        {
            unitTile.KillWithoutMove(board[targetPosition], board);
        }
    }
}