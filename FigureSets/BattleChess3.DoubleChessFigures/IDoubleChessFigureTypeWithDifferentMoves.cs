﻿using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.DoubleChessFigures;

internal interface IDoubleChessFigureTypeWithDifferentMoves : IDoubleChessFigureType
{
    /// <summary>
    ///     15 x 15 field
    ///     0 - no action
    ///     1 - possible move
    ///     2 - possible attack
    ///     3 - possible move + attack
    ///     8 - the unit
    /// </summary>
    protected int[] Actions { get; }

    FigureAction IFigureType.GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        var movement = targetTile.Position - unitTile.Position;
        var targetPosition = 7 - movement.X + (7 - movement.Y) * 15;

        if (targetTile.IsEmpty() && (Actions[targetPosition] & 1) == 1)
        {
            return unitTile.CreateMoveAction(targetTile, board);
        }

        if (targetTile.IsOwnedByYou(unitTile) && (Actions[targetPosition] & 1) == 1)
        {
            return CreateMergeAction(unitTile, targetTile, board);
        }

        if (targetTile.IsOwnedByEnemy(unitTile) && (Actions[targetPosition] & 2) == 2)
        {
            return unitTile.CreateKillWithMove(targetTile, board);
        }

        return FigureAction.None;
    }
}