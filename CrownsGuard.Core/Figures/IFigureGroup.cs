using CrownsGuard.Core.SimulatedBoard;

namespace CrownsGuard.Core.Figures;

public interface IFigureGroup
{
    /// <summary>
    ///     Shown name of figure group
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    ///     Figure types of group
    /// </summary>
    IFigureType[] FigureTypes { get; }

    /// <summary>
    /// Bets figure type by unique unit id.
    /// </summary>
    IFigureType GetFigureTypeById(FigureId uniqueUnitId);
}