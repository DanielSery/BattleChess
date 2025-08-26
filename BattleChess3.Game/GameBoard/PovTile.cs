using BattleChess3.Game.Figures;
using BattleChess3.Game.Helpers;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.GameBoard;

public class PovTile : ITile
{
    private readonly ITile _innerTile;
    private readonly Player _player;

    public PovTile(ITile innerTile, Player player)
    {
        _innerTile = innerTile;
        _player = player;
    }

    public Position Position => PlayerPositionHelper.GetPlayerPOVPosition(_player, _innerTile.Position);
    public Position AbsolutePosition => _innerTile.Position;

    public Figure Figure
    {
        get => _innerTile.Figure;
        set => _innerTile.Figure = value;
    }

    public ITile GetPovTile(Player player)
    {
        return new PovTile(this, player);
    }

    /// <inheritdoc />
    public void OnDied()
    {
        _innerTile.OnDied();
    }

    /// <inheritdoc />
    public void OnMovedTo()
    {
        _innerTile.OnMovedTo();
    }

    /// <inheritdoc />
    public void OnCreated()
    {
        _innerTile.OnCreated();
    }
}