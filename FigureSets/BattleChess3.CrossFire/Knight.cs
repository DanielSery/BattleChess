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

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        var movement = targetTile.Position - unitTile.Position;
        var targetPosition = 7 - movement.X + (7 - movement.Y) * 15;

        if (targetTile.IsEmpty() && (Actions[targetPosition] & 1) == 1)
        {
            return unitTile.CreateMoveAction(targetTile, board);
        }

        if (targetTile.IsOwnedByEnemy(unitTile) && (Actions[targetPosition] & 2) == 2)
        {
            return new FigureAction(FigureActionTypes.Attack, () =>
                AttackAction(unitTile, targetTile, board));
        }

        return FigureAction.None;
    }

    private void AttackAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        var move = targetTile.Position - unitTile.Position;

        if (Math.Abs(move.X) <= 1 &&
            Math.Abs(move.Y) <= 1)
        {
            unitTile.KillWithMove(targetTile, board);
        }
        else if (Math.Abs(move.X) <= 2 &&
                 Math.Abs(move.Y) <= 2)
        {
            var smallMove = new Position(Math.Sign(move.X), Math.Sign(move.Y));
            var sourcePosition = unitTile.Position;
            
            unitTile.KillWithMove(board[sourcePosition + smallMove], board);
            unitTile = board[sourcePosition + smallMove];
            if (!unitTile.Figure.FigureType.Equals(this))
                return;
           
            unitTile.KillWithMove(targetTile, board); 
        }
        else
        {
            var smallMove = new Position(Math.Sign(move.X), Math.Sign(move.Y));
            var sourcePosition = unitTile.Position;
            
            unitTile.KillWithMove(board[sourcePosition + smallMove], board);
            unitTile = board[sourcePosition + smallMove];
            if (!unitTile.Figure.FigureType.Equals(this))
                return;
            
            unitTile.KillWithMove(board[sourcePosition + 2 * smallMove], board);
            unitTile = board[sourcePosition + 2 * smallMove];
            if (!unitTile.Figure.FigureType.Equals(this))
                return;
            
            unitTile.KillWithMove(targetTile, board);
        }
    }
}