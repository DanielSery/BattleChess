#region Copyright FEI Company 2024
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
#endregion

using System.Collections;

namespace BattleChess3.Core.Model;

public class Board : IBoard
{
    private readonly ITile[] _povTiles;
    private readonly ITile[] _absoluteTiles;

    public Board(
        ITile[] absoluteTiles,
        ITile[] povTiles)
    {
        _absoluteTiles = absoluteTiles;
        _povTiles = povTiles;
    }

    public IEnumerator<ITile> GetEnumerator()
    {
        return _absoluteTiles.Cast<ITile>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _absoluteTiles.Length;

    public ITile this[int index] => _povTiles[index];

    public ITile GetAbsoluteTile(Position position)
    {
        return _absoluteTiles[position];
    }

    public ITile GetPovTile(Position position)
    {
        return _povTiles[position];
    }

    public bool TryGetAbsoluteTile(Position position, out ITile tile)
    {
        if (position.X < 0 ||
            position.X >= 8 ||
            position.Y < 0 ||
            position.Y >= 8)
        {
            tile = NoneTile.Instance;
            return false;
        }

        tile = GetAbsoluteTile(position);
        return true;
    }

    public bool TryGetPovTile(Position position, out ITile tile)
    {
        if (position.X < 0 ||
            position.X >= 8 ||
            position.Y < 0 ||
            position.Y >= 8)
        {
            tile = NoneTile.Instance;
            return false;
        }

        tile = GetPovTile(position);
        return true;
    }
}