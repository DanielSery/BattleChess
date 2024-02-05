using System;
using System.Collections.Generic;

namespace BattleChess3.Core.Model.Figures;

public class NoneFigure : IFigureType
{
    public static NoneFigure Instance { get; } = new();
    
    public string DisplayName => string.Empty;
    public string Description => string.Empty;
    public string UnitName => string.Empty;
    public Dictionary<int, Uri> ImageUris { get; } = new();
    
    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        return FigureAction.None;
    }
}
