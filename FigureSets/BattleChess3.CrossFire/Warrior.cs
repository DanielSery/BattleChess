using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Warrior : ICrossFireFigureType
{
    int IFigureType.FigureId => 21;
    
    private readonly Position[] _movePositions =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];

    private readonly Position[] _attackDirections =
    [
        new(-1, -1),
        new(-1, 0),
        new(-1, 1),
        new(0, -1),
        new(0, 1),
        new(1, -1),
        new(1, 0),
        new(1, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in _movePositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }
        
        foreach (var direction in _attackDirections)
        {
            for (var i = 1; i <= 3; i++)
            {
                if (!board.TryGetRelativeTile(unitTile, direction * i, out var targetTile))
                    break;

                if (unitTile.CanAttack(targetTile))
                {
                    yield return new FigureAction(
                        FigureActionTypes.Attack, 
                        unitTile.AbsolutePosition,
                        targetTile.AbsolutePosition,
                        () => AttackAction(unitTile, targetTile, board));
                }
                else if (!unitTile.CanMoveTo(targetTile))
                {
                    break;
                }
            }
        }
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
            if (!unitTile.Figure.Type.Equals(this))
                return;
           
            unitTile.KillWithMove(targetTile, board); 
        }
        else
        {
            var smallMove = new Position(Math.Sign(move.X), Math.Sign(move.Y));
            var sourcePosition = unitTile.Position;
            
            unitTile.KillWithMove(board[sourcePosition + smallMove], board);
            unitTile = board[sourcePosition + smallMove];
            if (!unitTile.Figure.Type.Equals(this))
                return;
            
            unitTile.KillWithMove(board[sourcePosition + 2 * smallMove], board);
            unitTile = board[sourcePosition + 2 * smallMove];
            if (!unitTile.Figure.Type.Equals(this))
                return;
            
            unitTile.KillWithMove(targetTile, board);
        }
    }
}