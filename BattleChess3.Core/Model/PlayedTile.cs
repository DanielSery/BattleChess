using BattleChess3.Core.Model.Figures;

namespace BattleChess3.Core.Model;

public class PlayedTile : ITile
{
    private readonly ITile _innerTile;
    private readonly Player _player;

    public PlayedTile(ITile innerTile, Player player)
    {
        _innerTile = innerTile;
        _player = player;
    }

    public Position Position => _innerTile.Position.GetPlayerPOVPosition(_player);
    public Position AbsolutePosition => _innerTile.Position;

    public Figure Figure
    {
        get => _innerTile.Figure;
        set => _innerTile.Figure = value;
    }

    public ITile GetPovTile(Player player)
    {
        return new PlayedTile(this, player);
    }
}