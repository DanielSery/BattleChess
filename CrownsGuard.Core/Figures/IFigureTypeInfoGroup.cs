namespace CrownsGuard.Core.Figures;

public interface IFigureTypeInfoGroup
{
    /// <summary>
    ///     Shown name of figure group
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    ///     Figure types of group
    /// </summary>
    IFigureTypeInfo[] FigureTypes { get; }

    /// <summary>
    /// Bets figure type by unique unit id.
    /// </summary>
    IFigureTypeInfo GetFigureTypeById(Figure uniqueUnit);
}