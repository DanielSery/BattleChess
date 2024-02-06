using BattleChess3.UI.Model;

namespace BattleChess3.UI.Services;

/// <summary>
///     Service for handling themes.
/// </summary>
public interface IThemeService
{
    /// <summary>
    ///     Raised when themes collection changes.
    /// </summary>
    event EventHandler<IList<ThemeModel>>? ThemesChanged;

    /// <summary>
    ///     Gets current themes.
    /// </summary>
    IList<ThemeModel> GetCurrentThemes();
}