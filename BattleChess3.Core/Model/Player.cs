using BattleChess3.Core.Model.Figures;

namespace BattleChess3.Core.Model;

public class Player : IEquatable<Player>
{
    public static readonly Player Neutral = new(0);
    
    public int Id { get; }
    public List<Figure> Figures { get; } = new();

    public Player(int id)
    {
        Id = id;
    }

    public override string ToString() => $"Player{Id}";

    public bool Equals(Player? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Player)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
