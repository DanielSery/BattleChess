﻿using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.DefaultFigures;

public class Palm : IDefaultFigureType
{
    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        return FigureAction.None;
    }
}