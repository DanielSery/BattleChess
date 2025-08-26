using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.GameBoard;

public interface ITile
{
    /// <summary>
    ///     Position of tile in board
    /// </summary>
    Position Position { get; }

    /// <summary>
    ///     Absolute position of tile in board.
    /// </summary>
    Position AbsolutePosition { get; }

    /// <summary>
    ///     Current figure on tile
    /// </summary>
    Figure Figure { get; set; }

    /// <summary>
    ///     Gets tile with position according to players point of view.
    /// </summary>
    ITile GetRelativeTile(PlayerInfo player);

    void OnDied();
    void OnMovedFrom();
    void OnMovedTo();
    void OnCreated();

    /// <summary>
    ///     ToString for debugging
    /// </summary>
    string? ToString()
    {
        return $"{Position}:{Figure}";
    }
}