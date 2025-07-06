namespace BattleChess3.Maps;

/// <summary>
///     Service for handling maps loading.
/// </summary>
public interface IMapService
{
    /// <summary>
    ///     Gets current maps.
    /// </summary>
    MapBlueprint GetCurrentMap();

    /// <summary>
    ///     Saves specified map.
    /// </summary>
    void Save(MapBlueprint map);
}