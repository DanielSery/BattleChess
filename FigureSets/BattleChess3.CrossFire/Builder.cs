using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.CrossFireFigures;

public class Builder : ICrossFireFigureType
{
    public static readonly Builder Instance = new();

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        var movement = targetTile.Position - unitTile.Position;
        var targetPosition = (7 - movement.X) + (7 - movement.Y) * 15;

        if (targetTile.IsEmpty() && (Actions[targetPosition] & 1) == 1)
        {
            return new FigureAction(FigureActionTypes.Move, () =>
                MoveAction(unitTile, targetTile, board));
        }

        return FigureAction.None;
    }

    public void MoveAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        var move = targetTile.Position - unitTile.Position;
        MoveFiguresOutsideShield(unitTile, move, board);
        MoveShield(unitTile, move, board);
        unitTile.MoveToTile(targetTile);
    }

    private void MoveShield(ITile sourceTile, Position move, ITile[] board)
    {
        foreach (var shieldPosition in _shieldPositions)
        {
            if (!(sourceTile.Position + shieldPosition).IsInBoard())
                continue;

            var shieldTile = board[sourceTile.Position + shieldPosition];
            if (shieldTile.Figure.UnitName == Wall.Instance.UnitName &&
                shieldTile.Figure.Owner.Equals(sourceTile.Figure.Owner))
            {
                shieldTile.Die();
            }
        }

        foreach (var shieldPosition in _shieldPositions)
        {
            if (!(sourceTile.Position + shieldPosition + move).IsInBoard())
                continue;

            var shieldTile = board[sourceTile.Position + shieldPosition + move];
            if (shieldTile.IsEmpty())
            {
                shieldTile.CreateFigure(new Figure(sourceTile.Figure.Owner, Wall.Instance));
            }
        }
    }

    private void MoveFiguresOutsideShield(ITile sourceTile, Position move, ITile[] board)
    {
        Position[] movedPositions;
        if (move == new Position(0, 1))
            movedPositions = _upMovedPositions;
        else if (move == new Position(1, 0))
            movedPositions = _rightMovedPositions;
        else if (move == new Position(0, -1))
            movedPositions = _bottomMovedPositions;
        else
            movedPositions = _leftMovedPositions;

        foreach (var movedPosition in movedPositions)
        {
            if (!(sourceTile.Position + movedPosition).IsInBoard())
                continue;

            var movedTile = board[sourceTile.Position + movedPosition];
            if (movedTile.IsEmpty() ||
                movedTile.Figure.Owner.Equals(sourceTile.Figure.Owner) ||
                movedTile.Figure.UnitName == Wall.Instance.UnitName)
                continue;

            if (!(sourceTile.Position + movedPosition + move).IsInBoard())
            {
                movedTile.Die();
                continue;
            }

            var moveTargetTile = board[sourceTile.Position + movedPosition + move];
            if (moveTargetTile.IsEmpty())
            {
                movedTile.MoveToTile(moveTargetTile);
            }
            else
            {
                moveTargetTile.Die();
                movedTile.MoveToTile(moveTargetTile);
            }
        }
    }

    private readonly Position[] _upMovedPositions =
    {
        new(-2, 2),
        new(-1, 3),
        new(0, 3),
        new(1, 3),
        new(2, 2),
    };

    private readonly Position[] _rightMovedPositions =
    {
        new(2, -2),
        new(3, -1),
        new(3, 0),
        new(3, 1),
        new(2, 2),
    };

    private readonly Position[] _bottomMovedPositions =
    {
        new(-2, -2),
        new(-1, -3),
        new(0, -3),
        new(1, -3),
        new(2, -2),
    };

    private readonly Position[] _leftMovedPositions =
    {
        new(-2, -2),
        new(-3, -1),
        new(-3, 0),
        new(-3, 1),
        new(-2, 2),
    };

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
        new(-1, 2),
    };

    private int[] Actions { get; } = {
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
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    };
}
