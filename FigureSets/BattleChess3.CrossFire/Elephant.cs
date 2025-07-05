using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Elephant : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 8;
    
    int IFigureType.FigureId => 41;
    
    private static readonly Position[] Directions =
    [
        new(0, 1), new(0, -1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var direction in Directions)
        {
            var isAttack = false;
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 3, board, unitTile))
            {
                if (!isAttack && unitTile.CanMoveTo(targetTile))
                {
                    yield return new FigureAction(
                        FigureActionTypes.Move, 
                        unitTile.AbsolutePosition,
                        targetTile.AbsolutePosition,
                        () => AttackAction(unitTile, targetTile, board));
                }
                else
                {
                    isAttack = true;
                    yield return new FigureAction(
                        FigureActionTypes.Attack, 
                        unitTile.AbsolutePosition,
                        targetTile.AbsolutePosition,
                        () => AttackAction(unitTile, targetTile, board));
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
            if (unitTile.CanMoveTo(targetTile))
                unitTile.MoveToTile(targetTile, board);
            else unitTile.KillWithMove(targetTile, board);
        }
        else if (Math.Abs(move.X) <= 2 &&
                 Math.Abs(move.Y) <= 2)
        {
            var smallMove = new Position(Math.Sign(move.X), Math.Sign(move.Y));
            var sourcePosition = unitTile.Position;
            var step1Tile = board[sourcePosition + smallMove];

            if (unitTile.CanMoveTo(step1Tile))
                unitTile.MoveToTile(step1Tile, board);
            else unitTile.KillWithMove(step1Tile, board);
            
            if (!step1Tile.Figure.Type.Equals(this))
                return;
           
            if (step1Tile.CanMoveTo(targetTile))
                step1Tile.MoveToTile(targetTile, board);
            else step1Tile.KillWithMove(targetTile, board);
        }
        else
        {
            var smallMove = new Position(Math.Sign(move.X), Math.Sign(move.Y));
            var sourcePosition = unitTile.Position;
            
            var step1Tile = board[sourcePosition + smallMove];
            if (unitTile.CanMoveTo(step1Tile))
                unitTile.MoveToTile(step1Tile, board);
            else unitTile.KillWithMove(step1Tile, board);
            
            if (!step1Tile.Figure.Type.Equals(this))
                return;
            
            var step2Tile = board[sourcePosition + 2 * smallMove];
            if (step1Tile.CanMoveTo(step2Tile))
                step1Tile.MoveToTile(step2Tile, board);
            else step1Tile.KillWithMove(step2Tile, board);
            
            if (!step2Tile.Figure.Type.Equals(this))
                return;
            
            if (step2Tile.CanMoveTo(targetTile))
                step2Tile.MoveToTile(targetTile, board);
            else step2Tile.KillWithMove(targetTile, board);
        }
    }
}