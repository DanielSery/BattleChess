﻿using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.DoubleChessFigures;

internal interface IDoubleChessFigureTypeWithDifferentAttackMoves : IDoubleChessFigureType
{
    protected Position[] AttackMovePositions { get; }

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movement in AttackMovePositions)
        {
            var position = unitTile.Position + movement;
            if (!board.TryGetTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsEmpty())
            {
                yield return unitTile.CreateMoveAction(targetTile, board);
            }

            if (targetTile.IsOwnedByEnemy(unitTile))
            {
                yield return unitTile.CreateKillWithMove(targetTile, board);
            }

            if (targetTile.IsOwnedByYou(unitTile) &&
                TryCreateMergeAction(unitTile, targetTile, board, out var mergeAction))
            {
                yield return mergeAction;
            }
        }
    }
}