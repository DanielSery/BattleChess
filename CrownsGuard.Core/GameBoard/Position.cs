using System.Diagnostics;

namespace CrownsGuard.Core.GameBoard;

[DebuggerDisplay("({X},{Y})")]
public readonly record struct Position
{
    public static readonly Position None = new(-1, -1);

    public readonly short X;
    public readonly short Y;

    public Position(short x, short y)
    {
        X = x;
        Y = y;
    }

    public int GetIndex() => Y * Constants.BoardLength + X;

    public static Position operator +(Position left, Position right)
    {
        return new Position((short)(left.X + right.X), (short)(left.Y + right.Y));
    }

    public static Position operator -(Position left, Position right)
    {
        return new Position((short)(left.X - right.X), (short)(left.Y - right.Y));
    }

    public static Position operator *(Position left, int right)
    {
        return new Position((short)(left.X * right), (short)(left.Y * right));
    }

    public static Position FromIndex(int index)
    {
        return new Position((short)(index % Constants.BoardLength), (short)(index / Constants.BoardLength));
    }
}