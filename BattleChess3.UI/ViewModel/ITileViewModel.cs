using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.UI.ViewModel;

/// <summary>
///     Interface for view model of tile.
/// </summary>
public interface ITileViewModel : ITile
{
    /// <summary>
    ///     Gets if mouse is over tile
    /// </summary>
    bool IsMouseOver { get; set; }

    /// <summary>
    ///     Gets if tile is selected
    /// </summary>
    bool IsSelected { get; set; }

    bool IsPossibleAttack { get; }

    bool IsPossibleMove { get; }

    bool IsPossibleSpecial { get; }

    /// <summary>
    ///     Gets possible figure action.
    /// </summary>
    FigureAction PossibleAction { get; set; }
}