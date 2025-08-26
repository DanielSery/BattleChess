using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;

namespace BattleChess3.CrossFireFigures;

public class King : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 5;
    
    int IFigureType.FigureId => 23;
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in ICrossFireFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);

            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }

        if (unitTile.Position.Y != 0 ||
            unitTile.AbsolutePosition.X != 4)
        {
            yield break;
        }
        
        var rook1Tile = board[new(0, 0)];
        if (rook1Tile.IsAllyTo(unitTile) &&
            board[new(1, 0)].IsEmpty() &&
            board[new(2, 0)].IsEmpty() &&
            board[new(3, 0)].IsEmpty())
        {
            yield return new FigureAction(
                FigureActionTypes.Special, 
                unitTile.AbsolutePosition,
                board[new(2, 0)].AbsolutePosition,
                () =>
                {
                    unitTile.MoveToTile(board[new(2, 0)], board);
                    rook1Tile.MoveToTile(board[new(3, 0)], board);
                });
        }

        var rook2Tile = board[new(7, 0)];
        if (rook2Tile.IsAllyTo(unitTile) &&
            board[new(5, 0)].IsEmpty() &&
            board[new(6, 0)].IsEmpty())
        {
            yield return new FigureAction(
                FigureActionTypes.Special,
                unitTile.AbsolutePosition,
                board[new(6, 0)].AbsolutePosition,
                () =>
                {
                    unitTile.MoveToTile(board[new(6, 0)], board);
                    rook2Tile.MoveToTile(board[new(5, 0)], board);
                });
        }
    }
}