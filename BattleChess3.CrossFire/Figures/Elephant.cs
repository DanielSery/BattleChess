using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.CrossFireFigures.Utilities;

namespace BattleChess3.CrossFireFigures.Figures;

public class Elephant : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 10;
    
    int IFigureType.FigureId => CrossFireFigureIds.ElephantId;
    
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
                        targetTile.AbsolutePosition,
                        () => AttackAction(unitTile, targetTile, board));
                }
                else
                {
                    isAttack = true;
                    yield return new FigureAction(
                        FigureActionTypes.Attack, 
                        targetTile.AbsolutePosition,
                        () => AttackAction(unitTile, targetTile, board));
                }
            }
        }
    }

    private void AttackAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        var move = targetTile.RelativePosition - unitTile.RelativePosition;

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
            var sourcePosition = unitTile.RelativePosition;
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
            var sourcePosition = unitTile.RelativePosition;
            
            var step1Tile = board[sourcePosition + smallMove];
            if (unitTile.CanMoveTo(step1Tile))
                unitTile.MoveToTile(step1Tile, board);
            else unitTile.KillWithMove(step1Tile, board);
            
            if (!step1Tile.Figure.Type.Equals(this))
                return;
            
            var step2Tile = board[sourcePosition + smallMove * 2];
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