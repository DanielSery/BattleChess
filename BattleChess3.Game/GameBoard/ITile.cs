using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.GameBoard;

public interface ITile
{
    /// <summary>
    ///     RelativePosition of tile in board
    /// </summary>
    Position RelativePosition { get; }

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
    ITile GetRelativeTile(Player player);

    void OnDied();
    void OnMovedFrom();
    void OnMovedTo();
    void OnCreated();
}