using System.Collections;
using CrownsGuard.Core;
using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.Game.GameBoard;

public class Board : IBoard
{
    private readonly ITile[] _tiles;

    public Board(ITile[] tiles)
    {
        _tiles = tiles;
    }

    public ITile this[Position position] => _tiles[position.GetIndex()];

    public IEnumerator<ITile> GetEnumerator()
    {
        return _tiles.Cast<ITile>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool HasTileOnPosition(Position position)
    {
        var index = position.GetIndex();
        if (index < 0 || index >= _tiles.Length)
            return false;

        return position.X is >= 0 and < Constants.BoardLength;
    }

    public bool TryGetTile(Position position, out ITile tile)
    {
        if (!HasTileOnPosition(position))
        {
            tile = NoneTile.Instance;
            return false;
        }

        tile = this[position];
        return true;
    }
}