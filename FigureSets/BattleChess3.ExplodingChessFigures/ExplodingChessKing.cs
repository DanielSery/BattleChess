using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.ExplodingChessFigures;

public class ExplodingChessKing : IExplodingChessFigureType
{
    private readonly Position[] _attackMovePositions = 
    {
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1)
    };
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movement in _attackMovePositions)
        {
            var position = unitTile.Position + movement;
            if (!board.TryGetPovTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsEmpty())
            {
                yield return unitTile.CreateMoveAction(targetTile, board);
            }

            if (targetTile.IsOwnedByEnemy(unitTile))
            {
                yield return unitTile.CreateKillWithMove(targetTile, board);
            }
        }

        if (unitTile.Position.Y != 0 ||
            unitTile.AbsolutePosition.X != 4)
        {
            yield break;
        }
        
        var rook1Tile = board.GetAbsoluteTile((0, 0));
        if (rook1Tile.Figure.FigureType.Equals(ExplodingChessFigureGroup.ExplodingRook) &&
            board.GetAbsoluteTile((1, 0)).IsEmpty() &&
            board.GetAbsoluteTile((2, 0)).IsEmpty() &&
            board.GetAbsoluteTile((3, 0)).IsEmpty())
        {
            yield return new FigureAction(
                FigureActionTypes.Special, 
                unitTile.AbsolutePosition,
                rook1Tile.AbsolutePosition,
                () =>
                {
                    unitTile.MoveToTile(board.GetAbsoluteTile((2, 0)), board);
                    rook1Tile.MoveToTile(board.GetAbsoluteTile((3, 0)), board);
                });
        }

        var rook2Tile = board.GetAbsoluteTile((7, 0));
        if (rook2Tile.Figure.FigureType.Equals(ExplodingChessFigureGroup.ExplodingRook) &&
            board.GetAbsoluteTile((5, 0)).IsEmpty() &&
            board.GetAbsoluteTile((6, 0)).IsEmpty())
        {
            yield return new FigureAction(
                FigureActionTypes.Special,
                unitTile.AbsolutePosition,
                rook2Tile.AbsolutePosition,
                () =>
                {
                    unitTile.MoveToTile(board.GetAbsoluteTile((6, 0)), board);
                    rook2Tile.MoveToTile(board.GetAbsoluteTile((5, 0)), board);
                });
        }
    }
}