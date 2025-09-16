using System.Collections;
using CrownsGuard.Core;

namespace CrownsGuard.Maps.GameBoard;

public class BoardInfo : IBoardInfo
{
    private readonly ITileInfo[] _tiles;

    public BoardInfo(ITileInfo[] tiles)
    {
        _tiles = tiles;
    }

    public ITileInfo this[Position position] => _tiles[position.GetIndex()];

    public IEnumerator<ITileInfo> GetEnumerator()
    {
        return _tiles.Cast<ITileInfo>().GetEnumerator();
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

    public bool TryGetTile(Position position, out ITileInfo tileInfo)
    {
        if (!HasTileOnPosition(position))
        {
            tileInfo = NoneTileInfo.Instance;
            return false;
        }

        tileInfo = this[position];
        return true;
    }
}