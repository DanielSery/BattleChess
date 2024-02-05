using System;
using System.Collections.Generic;

namespace BattleChess3.Core.Model.Figures;

public interface IFigureType : IEquatable<IFigureType>
{
    /// <summary>
    /// Name shown in menus and helps
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets description of unit
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets name of unit
    /// </summary>
    string UnitName { get; }
    
    /// <summary>
    /// Images of player with id
    /// </summary>
    Dictionary<int, Uri> ImageUris { get; }

    /// <summary>
    /// Gets possible action on tile.
    /// </summary>
    FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board);
    
    /// <summary>
    /// Default equality comparison is based on unique unit name.
    /// </summary>
    bool IEquatable<IFigureType>.Equals(IFigureType? other) => 
        UnitName == other?.UnitName; 
}
