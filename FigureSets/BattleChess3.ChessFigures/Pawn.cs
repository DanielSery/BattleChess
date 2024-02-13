﻿using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.ChessFigures;

public class Pawn : IChessFigureType
{
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        if (TryGetAttackAction(unitTile, board, (1, 1), out var attackAction1))
        {
            yield return attackAction1;
        }

        if (TryGetAttackAction(unitTile, board, (-1, 1), out var attackAction2))
        {
            yield return attackAction2;
        }

        if (TryGetMoveAction(unitTile, board, (0, 1), out var moveAction1))
        {
            yield return moveAction1;
        }
        else
        {
            yield break;
        }

        if (unitTile.Position.Y == 1 &&
            TryGetMoveAction(unitTile, board, (0, 2), out var moveAction2))
        {
            yield return moveAction2;
        }
    }

    private static bool TryGetAttackAction(ITile unitTile, IBoard board, Position relativePosition, out FigureAction action)
    {
        var attackPosition = unitTile.Position + relativePosition;
        if (!board.TryGetTile(attackPosition, out var targetTile) ||
            !targetTile.IsOwnedByEnemy(unitTile))
        {
            action = FigureAction.None;
            return false;
        }

        if (attackPosition.Y == 7)
        {
            action = new FigureAction(
                FigureActionTypes.Special,
                unitTile.AbsolutePosition,
                targetTile.AbsolutePosition,
                () =>
                {
                    unitTile.KillWithoutMove(targetTile, board);
                    targetTile.CreateFigure(new Figure(unitTile.Figure.Owner, ChessFigureGroup.Queen), board);
                    unitTile.Die(board);
                });
            return true;
        }

        action = unitTile.CreateKillWithMove(targetTile, board);
        return true;
    }

    private static bool TryGetMoveAction(ITile unitTile, IBoard board, Position relativePosition, out FigureAction action)
    {
        var movePosition = unitTile.Position + relativePosition;
        if (!board.TryGetTile(movePosition, out var targetTile) ||
            !targetTile.IsEmpty())
        {
            action = FigureAction.None;
            return false;
        }

        if (movePosition.Y == 7)
        {
            action = new FigureAction(
                FigureActionTypes.Special,
                unitTile.AbsolutePosition,
                targetTile.AbsolutePosition,
                () =>
                {
                    targetTile.CreateFigure(new Figure(unitTile.Figure.Owner, ChessFigureGroup.Queen), board);
                    unitTile.Die(board);
                });
            return true;
        }

        action = unitTile.CreateMoveAction(targetTile, board);
        return true;
    }
}