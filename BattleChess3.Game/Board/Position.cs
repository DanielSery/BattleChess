using BattleChess3.Game.Players;

namespace BattleChess3.Game.Board;

public readonly struct Position : IEquatable<Position>
{
    public static readonly Position None = new(-1, -1);
    public int X { get; }
    public int Y { get; }
    public int Index => Y * IBoard.Length + X;

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool IsInBoard()
    {
        return X >= 0
               && Y >= 0
               && X < IBoard.Length
               && Y < IBoard.Length;
    }

    public static bool operator ==(Position left, Position right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(Position left, Position right)
    {
        return left.X != right.X || left.Y != right.Y;
    }

    public static Position operator +(Position left, Position right)
    {
        return new Position(left.X + right.X, left.Y + right.Y);
    }

    public static Position operator -(Position left, Position right)
    {
        return new Position(left.X - right.X, left.Y - right.Y);
    }

    public static Position operator *(Position left, Position right)
    {
        return new Position(left.X * right.X, left.Y * right.Y);
    }

    public static Position operator *(Position left, int right)
    {
        return new Position(left.X * right, left.Y * right);
    }

    public static Position operator *(int left, Position right)
    {
        return new Position(left * right.X, left * right.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is Position pos && Equals(pos);
    }

    public bool Equals(Position other)
    {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (X * 397) ^ Y;
        }
    }

    public static Position FromIndex(int index)
    {
        return new Position(index % IBoard.Length, index / IBoard.Length);
    }

    public Position GetPlayerPOVPosition(in int playerId)
    {
        return playerId switch
        {
            1 => new Position(X, IBoard.Length - Y - 1),
            2 => new Position(X, Y),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public Position GetPlayerPOVPosition(in Player currentPlayer)
        => GetPlayerPOVPosition(currentPlayer.Index);

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    public override string ToString()
    {
        return $"({X},{Y})";
    }
}