using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.StarWarsFigures;

public class Special : IStarWarsFigureType
{
    private readonly Position[] _shieldPositions =
    {
        new(-2, 2),
        new(-2, 1),
        new(-2, 0),
        new(-2, -1),
        new(-2, -2),

        new(2, 2),
        new(2, 1),
        new(2, 0),
        new(2, -1),
        new(2, -2),

        new(2, -2),
        new(1, -2),
        new(0, -2),
        new(-1, -2),
        new(-2, -2),

        new(2, 2),
        new(1, 2),
        new(0, 2),
        new(-1, 2),
        new(-2, 2)
    };

    private int[] Actions { get; } =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 1, 8, 1, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        var movement = targetTile.Position - unitTile.Position;
        var targetPosition = 7 - movement.X + (7 - movement.Y) * 15;
        
        if (targetTile.Figure.FigureType is Bomb &&
            (Actions[targetPosition] & 1) == 1)
        {
            if (targetTile.IsOwnedByYou(unitTile))
            {
                return new FigureAction(FigureActionTypes.Move, () =>
                {
                    var move = targetTile.Position - unitTile.Position;
                    targetTile.Die(board);
                    MoveShield(unitTile, move, board);
                    unitTile.MoveToTile(targetTile, board);
                });
            }

            return new FigureAction(FigureActionTypes.Move, () =>
            {
                unitTile.KillWithMove(targetTile, board);
            });
        }

        if (targetTile.IsEmpty() && (Actions[targetPosition] & 1) == 1)
        {
            return new FigureAction(FigureActionTypes.Move, () =>
            {
                var move = targetTile.Position - unitTile.Position;
                MoveShield(unitTile, move, board);
                unitTile.MoveToTile(targetTile, board);
            });
        }

        return FigureAction.None;
    }

    private void MoveShield(ITile sourceTile, Position move, IBoard board)
    {
        foreach (var shieldPosition in _shieldPositions)
        {
            if (!(sourceTile.Position + shieldPosition).IsInBoard())
            {
                continue;
            }

            var shieldTile = board[sourceTile.Position + shieldPosition];
            if (shieldTile.Figure.FigureType is Bomb &&
                shieldTile.Figure.Owner.Equals(sourceTile.Figure.Owner))
            {
                shieldTile.Die(board);
            }
        }

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
                shieldTile.CreateFigure(new Figure(sourceTile.Figure.Owner, StarWarsFigureGroup.Bomb), board);
            }
        }
    }

    private static IEnumerable<Position> GetMovedPositions(Position move)
    {
        return move switch
        {
            (0, 1) => new Position[] { new(-2, 3), new(-1, 3), new(0, 3), new(1, 3), new(2, 3) },
            (1, 0) => new Position[] { new(3, -2), new(3, -1), new(3, 0), new(3, 1), new(3, 2) },
            (0, -1) => new Position[] { new(-2, -3), new(-1, -3), new(0, -3), new(1, -3), new(2, -3) },
            (-1, 0) => new Position[] { new(-3, -2), new(-3, -1), new(-3, 0), new(-3, 1), new(-3, 2) },
            _ => throw new ArgumentException($"Unexpected move of Builder {move}")
        };
    }
}