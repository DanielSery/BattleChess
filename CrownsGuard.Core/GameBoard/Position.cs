using System.Diagnostics;

namespace CrownsGuard.Core.GameBoard;

[DebuggerDisplay("({X},{Y})")]
public readonly record struct Position
{
    public static readonly Position None = new(-1, -1);

    public readonly sbyte X;
    public readonly sbyte Y;

    public Position(sbyte x, sbyte y)
    {
        X = x;
        Y = y;
    }

    public int GetIndex() => Y * Constants.BoardLength + X;

    public static Position operator +(Position left, Position right)
    {
        return new Position((sbyte)(left.X + right.X), (sbyte)(left.Y + right.Y));
    }

    public static Position operator -(Position left, Position right)
    {
        return new Position((sbyte)(left.X - right.X), (sbyte)(left.Y - right.Y));
    }

    public static Position operator *(Position left, int right)
    {
        return new Position((sbyte)(left.X * right), (sbyte)(left.Y * right));
    }

    public static Position FromIndex(int index)
    {
        return new Position((sbyte)(index % Constants.BoardLength), (sbyte)(index / Constants.BoardLength));
    }
}