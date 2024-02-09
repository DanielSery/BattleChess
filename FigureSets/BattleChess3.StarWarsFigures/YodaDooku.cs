using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.StarWarsFigures;

public class YodaDooku : IStarWarsFigureType
{
    private int[] Actions { get; } =
    {
        2, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 2,
        0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0,
        0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0,
        0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0,
        0, 0, 0, 0, 2, 0, 0, 2, 0, 0, 2, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 2, 1, 2, 1, 2, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 1, 2, 2, 2, 1, 0, 0, 0, 0, 0,
        2, 2, 2, 2, 2, 2, 2, 8, 2, 2, 2, 2, 2, 2, 2,
        0, 0, 0, 0, 0, 1, 2, 2, 2, 1, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 2, 1, 2, 1, 2, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 2, 0, 0, 2, 0, 0, 2, 0, 0, 0, 0,
        0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0,
        0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0,
        0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0,
        2, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 2
    };

    FigureAction IFigureType.GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        var movement = targetTile.Position - unitTile.Position;
        var movementUnit = new Position(Math.Sign(movement.X), Math.Sign(movement.Y));
        var targetPosition = 7 - movement.X + (7 - movement.Y) * 15;
        var checkedMovement = movementUnit;

        if (targetTile.IsOwnedByYou(unitTile) &&
            targetTile.Figure.FigureType is Bomb &&
            (Actions[targetPosition] & 1) == 1)
        {
            return new FigureAction(FigureActionTypes.Move, () =>
            {
                targetTile.Die(board);
                unitTile.MoveToTile(targetTile, board);
            });
        }

        if (targetTile.IsEmpty() && (Actions[targetPosition] & 1) == 1)
        {
            return unitTile.CreateMoveAction(targetTile, board);
        }

        var hasFoundTarget = false;
        var foundUnitPosition = new Position(-1, -1);
        for (var i = 0; i < 7; i++)
        {
            if (checkedMovement == movement)
            {
                hasFoundTarget = true;
            }

            if (hasFoundTarget && foundUnitPosition != new Position(-1, -1))
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

            if (!board[position].IsEmpty() && foundUnitPosition != new Position(-1, -1))
            {
                return FigureAction.None;
            }
            
            if (!board[position].IsEmpty() && !isYoursBomb)
            {
                foundUnitPosition = position;
            }

            checkedMovement += movementUnit;
        }

        if (foundUnitPosition == new Position(-1, -1))
        {
            return FigureAction.None;
        }

        if (targetTile.IsEmpty() && (Actions[targetPosition] & 2) == 2)
        {
            var movedTile = board[foundUnitPosition];
            if (movedTile.Figure.FigureType is Bomb)
            {
                return new FigureAction(FigureActionTypes.Attack, () =>
                {
                    var figureType = movedTile.Figure.FigureType;
                    movedTile.Figure.Owner.Figures.Remove(targetTile.Figure);
                    movedTile.Figure = new Figure(unitTile.Figure.Owner, figureType);
                    targetTile.SwapTiles(board[foundUnitPosition]);
                });
            }
            else
            {
                return new FigureAction(FigureActionTypes.Special, () =>
                {
                    targetTile.SwapTiles(board[foundUnitPosition]);
                });
            }
        }
        
        if (targetTile.IsOwnedByYou(unitTile) &&
            targetTile.Figure.FigureType is Bomb &&
            (Actions[targetPosition] & 1) == 1)
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