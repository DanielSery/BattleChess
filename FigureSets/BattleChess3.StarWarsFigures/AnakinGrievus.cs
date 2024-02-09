using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
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
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 1, 0, 8, 0, 1, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 3, 0, 1, 0, 3, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    void IFigureType.OnMoved(ITile unitTile, ITile targetTile, IBoard board)
    {
        var movement = targetTile.Position - unitTile.Position;
        if (Math.Abs(movement.X) == Math.Abs(movement.Y))
        {
            TryDestroyTile(targetTile, board, (1, 0));
            TryDestroyTile(targetTile, board, (-1, 0));
            TryDestroyTile(targetTile, board, (0, 1));
            TryDestroyTile(targetTile, board, (0, -1));
        }
        else
        {
            TryDestroyTile(targetTile, board, (1, -1));
            TryDestroyTile(targetTile, board, (-1, 1));
            TryDestroyTile(targetTile, board, (1, 1));
            TryDestroyTile(targetTile, board, (-1, -1));
        }
    }

    private static void TryDestroyTile(ITile unitTile, IBoard board, Position positionDiff)
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