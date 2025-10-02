using CrownsGuard.Core.Figures;

namespace CrownsGuard.Maps.BoardBlueprints;

/// <summary>
///     Service for handling maps loading.
/// </summary>
public interface ISetupLoadingService
{
    Figure[] CurrentMap { get; }

    /// <summary>
    ///     Saves specified map.
    /// </summary>
    void Save(Figure[] map);
}