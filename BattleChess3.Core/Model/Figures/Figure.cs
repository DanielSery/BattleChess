using System;
using System.Collections.Generic;

namespace BattleChess3.Core.Model.Figures;

public class Figure : IFigureType
{
    public static readonly Figure None = new(Player.Neutral, NoneFigure.Instance);
    
    public Player Owner { get; }
    public IFigureType FigureType { get; set; }
    public Uri ImageUri => FigureType.ImageUris[Owner.Id];

    public string DisplayName => FigureType.DisplayName;
    public string Description => FigureType.Description;
    public string UnitName => FigureType.UnitName;
    public IDictionary<int, Uri> ImageUris => FigureType.ImageUris;

    public Figure(Player owner, IFigureType figureType)
    {
        Owner = owner;
        FigureType = figureType;
    }

    public FigureAction GetPossibleAction(ITile from, ITile to, ITile[] board)
        => FigureType.GetPossibleAction(from, to, board);
    
    public override string ToString() => $"{FigureType.DisplayName}:{Owner.Id}";
}
