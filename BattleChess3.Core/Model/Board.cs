#region Copyright FEI Company 2024
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
#endregion

using System.Collections;

namespace BattleChess3.Core.Model;

public class Board : IBoard
{
    private readonly ITile[] _tiles;

    public Board(ITile[] tiles)
    {
        _tiles = tiles;
    }

    public IEnumerator<ITile> GetEnumerator()
    {
        return _tiles.Cast<ITile>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _tiles.Length;

    public ITile this[int index] => _tiles[index];
    public ITile this[Position position] => _tiles[position];
}