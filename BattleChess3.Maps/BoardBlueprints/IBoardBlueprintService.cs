
using BattleChess3.Core.GameBoard;

namespace BattleChess3.Maps.BoardBlueprints;

/// <summary>
///     Service for handling maps loading.
/// </summary>
public interface IBoardBlueprintService
{
    BoardBlueprint CurrentMap { get; }

    /// <summary>
    ///     Saves specified map.
    /// </summary>
    void Save(BoardBlueprint map);
}