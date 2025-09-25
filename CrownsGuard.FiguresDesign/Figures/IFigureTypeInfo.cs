using CrownsGuard.Core.Figures;

namespace CrownsGuard.FiguresDesign.Figures;

public interface IFigureTypeInfo
{
    Figure Figure { get; }

    /// <summary>
    ///     Name shown in menus and helps
    /// </summary>
    string DisplayName { get; }
    
    /// <summary>
    ///     Gets base description of unit 
    /// </summary>
    string BaseDescription { get; }

    /// <summary>
    ///     Gets description of unit movement
    /// </summary>
    string MovementDescription { get; }

    /// <summary>
    ///     Gets description of unit attack
    /// </summary>
    string AttackDescription { get; }

    /// <summary>
    ///     Gets description of unit special abilities
    /// </summary>
    string SpecialDescription { get; }
    
    /// <summary>
    ///     Images of player with id
    /// </summary>
    IDictionary<int, Uri> ImageUris { get; }
}