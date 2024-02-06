using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.CrossFireFigures;

public class Knight : ICrossFireFigureType
{
    private int[] Actions { get; } =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 2, 0, 0, 2, 0, 0, 2, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 2, 1, 2, 1, 2, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 1, 2, 2, 2, 1, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 2, 2, 2, 8, 2, 2, 2, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 1, 2, 2, 2, 1, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 2, 1, 2, 1, 2, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 2, 0, 0, 2, 0, 0, 2, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        var movement = targetTile.Position - unitTile.Position;
        var targetPosition = 7 - movement.X + (7 - movement.Y) * 15;

        if (targetTile.IsEmpty() && (Actions[targetPosition] & 1) == 1)
        {
            return unitTile.CreateMoveAction(targetTile);
        }

        if (targetTile.IsOwnedByEnemy(unitTile) && (Actions[targetPosition] & 2) == 2)
        {
            return new FigureAction(FigureActionTypes.Attack, () =>
                AttackAction(unitTile, targetTile, board));
        }

        return FigureAction.None;
    }

    private static void AttackAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        var move = targetTile.Position - unitTile.Position;

        if (Math.Abs(move.X) <= 1 &&
            Math.Abs(move.Y) <= 1)
        {
            targetTile.Die();
            unitTile.MoveToTile(targetTile);
        }
        else if (Math.Abs(move.X) <= 2 &&
                 Math.Abs(move.Y) <= 2)
        {
            var smallMove = new Position(Math.Sign(move.X), Math.Sign(move.Y));
            board[unitTile.Position + smallMove].Die();
            targetTile.Die();
            unitTile.MoveToTile(targetTile);
        }
        else
        {
            var smallMove = new Position(Math.Sign(move.X), Math.Sign(move.Y));
            board[unitTile.Position + smallMove].Die();
            board[unitTile.Position + 2 * smallMove].Die();
            targetTile.Die();
            unitTile.MoveToTile(targetTile);
        }
    }
}