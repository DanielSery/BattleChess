using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.DoubleChessFigures;

public class SingleKing : IDoubleChessFigureType
{
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
                targetTile.Figure.FigureType.Equals(DoubleChessFigureGroup.Rook) &&
                board[new Position(1, 0)].IsEmpty() &&
                board[new Position(2, 0)].IsEmpty() &&
                board[new Position(3, 0)].IsEmpty())
            {
                return new FigureAction(FigureActionTypes.Special, () =>
                {
                    unitTile.MoveToTile(board[new Position(2, 0)], board);
                    targetTile.MoveToTile(board[new Position(3, 0)], board);
                });
            }

            if (targetTile.AbsolutePosition.X == 7 &&
                targetTile.Figure.FigureType.Equals(DoubleChessFigureGroup.Rook) &&
                board[new Position(5, 0)].IsEmpty() &&
                board[new Position(6, 0)].IsEmpty())
            {
                return new FigureAction(FigureActionTypes.Special, () =>
                {
                    unitTile.MoveToTile(board[new Position(6, 0)], board);
                    targetTile.MoveToTile(board[new Position(5, 0)], board);
                });
            }
        }

        if (targetTile.IsEmpty() && (Actions[targetPosition] & 1) == 1)
        {
            return unitTile.CreateMoveAction(targetTile, board);
        }

        if (targetTile.IsOwnedByYou(unitTile) && (Actions[targetPosition] & 1) == 1)
        {
            return (this as IDoubleChessFigureType).CreateMergeAction(unitTile, targetTile, board);
        }

        if (targetTile.IsOwnedByEnemy(unitTile) && (Actions[targetPosition] & 2) == 2)
        {
            return unitTile.CreateKillWithMove(targetTile, board);
        }

        return FigureAction.None;
    }
}