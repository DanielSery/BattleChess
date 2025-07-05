using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class YoungWizzard : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 20;
    
    int IFigureType.FigureId => 14;
    
    private static readonly Position[] MovementPositions =
    [
        new(-2, -2), new(-2, 0), new(-2, 2),
        new(0, -2), new(0, 2),
        new(2, -2), new(2, 0), new(2, 2)
    ];

    private static readonly Position[] AttackPositions =
    [
        new(-2, -2), new(-2, 2),
        new(2, -2), new(2, 2)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovementPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }

        foreach (var targetTile in AttackPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }
    }

    void IFigureType.OnMoved(ITile unitTile, ITile targetTile, IBoard board)
    {
        var movement = targetTile.Position - unitTile.Position;
        if (Math.Abs(movement.X) == Math.Abs(movement.Y))
        {
            targetTile.TryDestroyTile(board, new Position(1, 0));
            targetTile.TryDestroyTile(board, new Position(-1, 0));
            targetTile.TryDestroyTile(board, new Position(0, 1));
            targetTile.TryDestroyTile(board, new Position(0, -1));
        }
        else
        {
            targetTile.TryDestroyTile(board, new Position(1, -1));
            targetTile.TryDestroyTile(board, new Position(-1, 1));
            targetTile.TryDestroyTile(board, new Position(1, 1));
            targetTile.TryDestroyTile(board, new Position(-1, -1));
        }
    }
}