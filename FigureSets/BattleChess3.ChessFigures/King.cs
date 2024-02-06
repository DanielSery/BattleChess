using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.ChessFigures;

public class King : IChessFigureType
{
    public static readonly King Instance = new();

    private int[] Actions { get; } =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 3, 3, 3, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 3, 8, 3, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 3, 3, 3, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    FigureAction IFigureType.GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        var movement = targetTile.Position - unitTile.Position;
        var targetPosition = 7 - movement.X + (7 - movement.Y) * 15;

        if (unitTile.Position.Y == 0 &&
            unitTile.AbsolutePosition.X == 4 &&
            targetTile.Position.Y == 0)
        {
            if (targetTile.AbsolutePosition.X == 0 &&
                targetTile.Figure.FigureType.Equals(Rook.Instance) &&
                board[new Position(1, 0)].IsEmpty() &&
                board[new Position(2, 0)].IsEmpty() &&
                board[new Position(3, 0)].IsEmpty())
                return new FigureAction(FigureActionTypes.Special, () =>
                {
                    unitTile.MoveToTile(board[new Position(2, 0)]);
                    targetTile.MoveToTile(board[new Position(3, 0)]);
                });

            if (targetTile.AbsolutePosition.X == 7 &&
                targetTile.Figure.FigureType.Equals(Rook.Instance) &&
                board[new Position(5, 0)].IsEmpty() &&
                board[new Position(6, 0)].IsEmpty())
                return new FigureAction(FigureActionTypes.Special, () =>
                {
                    unitTile.MoveToTile(board[new Position(6, 0)]);
                    targetTile.MoveToTile(board[new Position(5, 0)]);
                });
        }

        if (targetTile.IsEmpty() && (Actions[targetPosition] & 1) == 1) return unitTile.CreateMoveAction(targetTile);

        if (targetTile.IsOwnedByEnemy(unitTile) && (Actions[targetPosition] & 2) == 2)
            return unitTile.CreateKillWithMove(targetTile);

        return FigureAction.None;
    }
}