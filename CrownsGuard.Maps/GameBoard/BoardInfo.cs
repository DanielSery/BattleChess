using System.Collections;

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
}