using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.StarWarsFigures;

public class Special : IStarWarsFigureType
{
    private readonly Position[] _shieldPositions =
    {
        new(-2, 1),
        new(-2, 0),
        new(-2, -1),

        new(2, 1),
        new(2, 0),
        new(2, -1),

        new(1, -2),
        new(0, -2),
        new(-1, -2),

        new(1, 2),
        new(0, 2),
        new(-1, 2)
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

        if (targetTile.IsEmpty() && (Actions[targetPosition] & 1) == 1)
        {
            return new FigureAction(FigureActionTypes.Move, () =>
                MoveAction(unitTile, targetTile, board));
        }

        return FigureAction.None;
    }

    private void MoveAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        var move = targetTile.Position - unitTile.Position;
        MoveFiguresOutsideShield(unitTile, move, board);
        MoveShield(unitTile, move, board);
        unitTile.MoveToTile(targetTile, board);
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

        foreach (var shieldPosition in _shieldPositions)
        {
            if (!(sourceTile.Position + shieldPosition + move).IsInBoard())
            {
                continue;
            }

            var shieldTile = board[sourceTile.Position + shieldPosition + move];
            if (shieldTile.IsEmpty())
            {
                shieldTile.CreateFigure(new Figure(sourceTile.Figure.Owner, StarWarsFigureGroup.Bomb), board);
            }
        }
    }

    private static void MoveFiguresOutsideShield(ITile sourceTile, Position move, IBoard board)
    {
        var movedPositions = GetMovedPositions(move);
        foreach (var movedPosition in movedPositions)
        {
            if (!(sourceTile.Position + movedPosition).IsInBoard())
            {
                continue;
            }

            var movedTile = board[sourceTile.Position + movedPosition];
            if (movedTile.IsEmpty() ||
                movedTile.Figure.FigureType is Bomb)
            {
                continue;
            }

            if (!(sourceTile.Position + movedPosition + move).IsInBoard())
            {
                movedTile.Die(board);
                continue;
            }

            var moveTargetTile = board[sourceTile.Position + movedPosition + move];
            if (moveTargetTile.IsEmpty())
            {
                movedTile.MoveToTile(moveTargetTile, board);
            }
            else
            {
                movedTile.KillWithMove(moveTargetTile, board);
            }
        }
    }

    private static IEnumerable<Position> GetMovedPositions(Position move)
    {
        return move switch
        {
            (0, 1) => new Position[] { new(-2, 2), new(-1, 3), new(0, 3), new(1, 3), new(2, 2) },
            (1, 0) => new Position[] { new(2, -2), new(3, -1), new(3, 0), new(3, 1), new(2, 2) },
            (0, -1) => new Position[] { new(-2, -2), new(-1, -3), new(0, -3), new(1, -3), new(2, -2) },
            (-1, 0) => new Position[] { new(-2, -2), new(-3, -1), new(-3, 0), new(-3, 1), new(-2, 2) },
            _ => throw new ArgumentException($"Unexpected move of Builder {move}")
        };
    }
}