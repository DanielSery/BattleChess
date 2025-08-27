using BattleChess3.Game.Figures;

namespace BattleChess3.Maps;

/// <summary>
///     Figure service is used loading figure dlls and resolving it's figures.
/// </summary>
public interface IFigureService
{
    /// <summary>
    ///     Gets all figure groups.
    /// </summary>
    IList<IFigureGroup> FigureGroups { get; }

    /// <summary>
    ///     Gets figure type based on name of the figure.
    /// </summary>
    IFigureType GetFigureByUniqueUnitId(int id);
}