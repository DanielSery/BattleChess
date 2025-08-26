using BattleChess3.Game.Figures;
using BattleChess3.Game.Helpers;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.GameBoard;

public class RelativeTile : ITile
{
    private readonly ITile _innerTile;
    private readonly PlayerInfo _playerInfo;

    public RelativeTile(ITile innerTile, PlayerInfo player)
    {
        _innerTile = innerTile;
        _playerInfo = player;
    }

    public Position Position => PlayerPositionHelper.GetPlayerRelativePosition(_playerInfo, _innerTile.AbsolutePosition);
    public Position AbsolutePosition => _innerTile.Position;

    public Figure Figure
    {
        get => _innerTile.Figure;
        set => _innerTile.Figure = value;
    }

    public ITile GetRelativeTile(PlayerInfo player)
    {
        return new RelativeTile(this, player);
    }

    /// <inheritdoc />
    public void OnDied()
    {
        _innerTile.OnDied();
    }

    public void OnMovedFrom()
    {
        _innerTile.OnMovedFrom();
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