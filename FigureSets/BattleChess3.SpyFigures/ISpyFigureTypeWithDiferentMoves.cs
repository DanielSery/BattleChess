﻿using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.SpyFigures;

internal interface ISpyFigureTypeWithDifferentAttackMoves : IFigureType
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
            else
            {
                yield return unitTile.CreateKillWithMove(targetTile, board);
            }
        }
    }
}