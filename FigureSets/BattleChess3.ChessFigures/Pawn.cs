using System;
using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.ChessFigures;

public class Pawn : IChessFigureType
{
    public static readonly Pawn Instance = new();

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        return CreateFigureAction(unitTile, targetTile, board,
            unitTile.Position.Y == 1 ? StartingActions : NormalActions);
    }

    private static FigureAction CreateFigureAction(ITile unitTile, ITile targetTile, ITile[] board, int[] actions)
    {
        var movement = targetTile.Position - unitTile.Position;
        var movementUnit = new Position(Math.Sign(movement.X), Math.Sign(movement.Y));
        var targetPosition = (7 - movement.X) + (7 - movement.Y) * 15;
        var checkedMovement = movementUnit;

        for (var i = 0; i < 7; i++)
        {
            if (checkedMovement == movement)
                break;

            var position = unitTile.Position + checkedMovement;
            if (position.IsOutsideBoard() || !board[position].IsEmpty())
                return FigureAction.None;

            checkedMovement += movementUnit;
        }
        
        if (targetTile.IsEmpty() && (actions[targetPosition] & 1) == 1)
        {
            if (targetTile.Position.Y == 7)
            {
                return new FigureAction(FigureActionTypes.Special, () =>
                {
                    targetTile.CreateFigure(new Figure(unitTile.Figure.Owner, Queen.Instance));
                    unitTile.Die();
                });
            }
            
            return unitTile.CreateMoveAction(targetTile);
        }

        if (targetTile.IsOwnedByEnemy(unitTile) && (actions[targetPosition] & 2) == 2)
        {
            if (targetTile.Position.Y == 7)
            {
                return new FigureAction(FigureActionTypes.Special, () =>
                {
                    targetTile.Die();
                    targetTile.CreateFigure(new Figure(unitTile.Figure.Owner, Queen.Instance));
                    unitTile.Die();
                });
            }
            
            return unitTile.CreateKillWithMove(targetTile);
        }

        return FigureAction.None;
    }

    private int[] StartingActions { get; } =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 2, 1, 2, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    };

    private int[] NormalActions { get; } =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 2, 1, 2, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    };
}