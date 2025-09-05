using System.Diagnostics;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Game.Helpers;

namespace CrownsGuard.Game.GameBoard;

[DebuggerDisplay("{RelativePosition}:{Figure}")]
public class RelativeTile : ITile
{
    private readonly ITile _innerTile;
    private readonly Player _player;

    public RelativeTile(ITile innerTile, Player player)
    {
        _innerTile = innerTile;
        _player = player;
    }

    public Position RelativePosition => RelativePositionHelper.GetRelative(_player, _innerTile.AbsolutePosition);
    public Position AbsolutePosition => _innerTile.AbsolutePosition;

    public IFigure Figure
    {
        get => _innerTile.Figure;
        set => _innerTile.Figure = value;
    }

    public ITile GetRelativeTile(Player player)
    {
        return new RelativeTile(_innerTile, player);
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