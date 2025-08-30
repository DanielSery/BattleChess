using BattleChess3.Game.GameBoard;

namespace BattleChess3.Maps;

/// <summary>
///     Service for handling maps loading.
/// </summary>
public interface IMapService
{
    /// <summary>
    ///     Gets current maps.
    /// </summary>
    BoardBlueprint GetCurrentMap();

    /// <summary>
    ///     Saves specified map.
    /// </summary>
    void Save(BoardBlueprint map);
}