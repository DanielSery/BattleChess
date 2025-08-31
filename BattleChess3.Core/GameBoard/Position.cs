using System.Diagnostics;

namespace BattleChess3.Core.GameBoard;

[DebuggerDisplay("({X},{Y})")]
public readonly record struct Position
{
    public static readonly Position None = new(-1, -1);

    public int X { get; }
    public int Y { get; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int GetIndex() => Y * Constants.BoardLength + X;

    public static Position operator +(Position left, Position right)
    {
        return new Position(left.X + right.X, left.Y + right.Y);
    }

    public static Position operator -(Position left, Position right)
    {
        return new Position(left.X - right.X, left.Y - right.Y);
    }

    public static Position operator *(Position left, int right)
    {
        return new Position(left.X * right, left.Y * right);
    }

    public static Position FromIndex(int index)
    {
        return new Position(index % Constants.BoardLength, index / Constants.BoardLength);
    }
}