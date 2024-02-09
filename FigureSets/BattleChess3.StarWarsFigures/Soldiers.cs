using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.StarWarsFigures;

public class Soldiers : IStarWarsFigureType 
{
    private int[] StartingActions { get; } =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 2, 0, 3, 0, 2, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 3, 1, 3, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    private int[] NormalActions { get; } =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 2, 0, 2, 0, 2, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 3, 1, 3, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };


    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        return CreateFigureAction(unitTile, targetTile, board,
            unitTile.Position.Y == 1 ? StartingActions : NormalActions);
    }

    private static FigureAction CreateFigureAction(ITile unitTile, ITile targetTile, IBoard board, int[] actions)
    {
        var movement = targetTile.Position - unitTile.Position;
        var movementUnit = new Position(Math.Sign(movement.X), Math.Sign(movement.Y));
        var targetPosition = 7 - movement.X + (7 - movement.Y) * 15;
        var checkedMovement = movementUnit;

        for (var i = 0; i < 7; i++)
        {
            if (checkedMovement == movement)
            {
                break;
            }
            
            var position = unitTile.Position + checkedMovement;
            if (position.IsOutsideBoard())
            {
                return FigureAction.None;
            }

            var isYoursBomb = board[position].Figure.FigureType is Bomb &&
                              board[position].IsOwnedByYou(unitTile);
            if (!board[position].IsEmpty() && !isYoursBomb)
            {
                return FigureAction.None;
            }

            checkedMovement += movementUnit;
        }

        if (targetTile.IsEmpty() && (actions[targetPosition] & 1) == 1)
        {
            if (targetTile.Position.Y == 7)
            {
                return new FigureAction(FigureActionTypes.Special, () =>
                {
                    targetTile.CreateFigure(new Figure(unitTile.Figure.Owner, StarWarsFigureGroup.ObiwanPalpatine), board);
                    unitTile.Die(board);
                });
            }

            return unitTile.CreateMoveAction(targetTile, board);
        }

        if (targetTile.IsOwnedByEnemy(unitTile) && (actions[targetPosition] & 2) == 2)
        {
            if (targetTile.Position.Y == 7)
            {
                return new FigureAction(FigureActionTypes.Special, () =>
                {
                    unitTile.KillWithoutMove(targetTile, board);
                    targetTile.CreateFigure(new Figure(unitTile.Figure.Owner, StarWarsFigureGroup.ObiwanPalpatine), board);
                    unitTile.Die(board);
                });
            }

            return unitTile.CreateKillWithMove(targetTile, board);
        }

        if (targetTile.IsOwnedByYou(unitTile) &&
            targetTile.Figure.FigureType is Bomb &&
            (actions[targetPosition] & 1) == 1)
        {
            return new FigureAction(FigureActionTypes.Move, () =>
            {
                targetTile.Die(board);
                unitTile.MoveToTile(targetTile, board);
            });
        }

        return FigureAction.None;
    }
}