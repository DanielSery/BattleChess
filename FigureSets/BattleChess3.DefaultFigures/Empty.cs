﻿using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.DefaultFigures;

public class Empty : IDefaultFigureType
{
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return Array.Empty<FigureAction>();
    }
}