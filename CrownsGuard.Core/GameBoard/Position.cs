using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CrownsGuard.Core.GameBoard;

[DebuggerDisplay("({X},{Y})")]
public readonly record struct Position
{
    private static readonly Position[] IndexCache = new Position[Constants.FullBoardTilesCount];
    public static readonly Position None = new(-1, -1);

    public readonly sbyte X;
    public readonly sbyte Y;

    static Position()
    {
        for (var i = 0; i < Constants.FullBoardTilesCount; ++i)
        {
            IndexCache[i] = new Position((sbyte)(i % Constants.BoardLength), (sbyte)(i / Constants.BoardLength));
        }
    }

    public Position(sbyte x, sbyte y)
    {
        X = x;
        Y = y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIndex() => Y * Constants.BoardLength + X;

    public static Position FromIndex(int index)
    {
        return IndexCache[index];
    }
}