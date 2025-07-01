using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures;

public class Miner : ICrossFireFigureType
{
    int IFigureType.FigureId => 29;
    
    private readonly Position[] _movePosition =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];
    
    private readonly Position[] _shieldPositions =
    [
        new(-2, 1), new(-2, 0), new(-2, -1),
        new(2, 1), new(2, 0), new(2, -1),
        new(1, -2), new(0, -2), new(-1, -2),
        new(1, 2), new(0, 2), new(-1, 2)
    ];
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movement in _movePosition)
        {
            var position = unitTile.Position + movement;
            if (!board.TryGetTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsEmpty())
            {
                yield return new FigureAction(
                    FigureActionTypes.Move, 
                    unitTile.AbsolutePosition,
                    targetTile.AbsolutePosition,
                    () =>
                    {
                        MoveShield(unitTile, movement, board);
                        unitTile.MoveToTile(targetTile, board);
                    });
            }

            if (targetTile.Figure.Type is Trench)
            {
                yield return new FigureAction(
                    FigureActionTypes.Move,
                    unitTile.AbsolutePosition,
                    targetTile.AbsolutePosition,
                    () =>
                    {
                        targetTile.Die(board);
                        MoveShield(unitTile, movement, board);
                        unitTile.MoveToTile(targetTile, board);
                    });
            }
        }
    }

    private void MoveShield(ITile sourceTile, Position move, IBoard board)
    {
        // foreach (var shieldPosition in _shieldPositions)
        // {
        //     if (!(sourceTile.Position + shieldPosition).IsInBoard())
        //     {
        //         continue;
        //     }
        //
        //     var shieldTile = board[sourceTile.Position + shieldPosition];
        //     if (shieldTile.Figure.Type is Trench)
        //     {
        //         shieldTile.Die(board);
        //     }
        // }

        var movedPositions = GetMovedPositions(move);
        foreach (var movedPosition in movedPositions)
        {
            if (!(sourceTile.Position + movedPosition).IsInBoard())
            {
                continue;
            }

            var shieldTile = board[sourceTile.Position + movedPosition];
            if (shieldTile.IsEmpty())
            {
                shieldTile.CreateFigure(new Figure(Player.Neutral, CrossFireFigureGroup.Trench), board);
            }
        }
    }

    private static IEnumerable<Position> GetMovedPositions(Position move)
    {
        return move switch
        {
            (0, 1) => new Position[] { new(-1, 2), new(0, 2), new(1, 2) },
            (1, 0) => new Position[] { new(2, -1), new(2, 0), new(2, 1) },
            (0, -1) => new Position[] { new(-1, -2), new(0, -2), new(1, -2) },
            (-1, 0) => new Position[] { new(-2, -1), new(-2, 0), new(-2, 1) },
            _ => throw new ArgumentException($"Unexpected move of Builder {move}")
        };
    }
}