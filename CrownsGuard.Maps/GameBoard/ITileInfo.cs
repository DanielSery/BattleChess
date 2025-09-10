using CrownsGuard.Core.GameBoard;
using CrownsGuard.Maps.Figures;

namespace CrownsGuard.Maps.GameBoard;

public interface ITileInfo
{
    /// <summary>
    ///     Absolute position of tile in board.
    /// </summary>
    Position Position { get; }

    /// <summary>
    ///     Current figure on tile
    /// </summary>
    IFigureWithInfo Figure { get; set; }
}