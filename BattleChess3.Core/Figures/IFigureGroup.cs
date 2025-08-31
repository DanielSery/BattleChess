namespace BattleChess3.Core.Figures;

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
}