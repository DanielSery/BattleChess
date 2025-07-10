using System.Diagnostics;
using BattleChess3.Game.Figures;

namespace BattleChess3.Game.Players;

public class Player : IEquatable<Player>
{
    public static readonly Player Neutral = new(null, "Neutral", 0, 0);

    public Player(string? playerId, string playerName, int? elo, int index)
    {
        Index = index;
        PlayerId = playerId;
        Name = playerName;
        Elo = elo;
        CurrentStopwatch = new Stopwatch();
        RemainingTime = TimeSpan.FromMinutes(5);
    }

    public int Index { get; }
    public string? PlayerId { get; set; }
    public string Name { get; set; }
    public int? Elo { get; set; }
    public List<Figure> Figures { get; } = [];
    public Stopwatch CurrentStopwatch { get; }
    public TimeSpan RemainingTime { get; private set; }

    public bool Equals(Player? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Index == other.Index;
    }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((Player)obj);
    }

    public override int GetHashCode()
    {
        return Index;
    }

    public void StartTurn()
    {
        RemainingTime += TimeSpan.FromSeconds(10);
        CurrentStopwatch.Start();
    }

    public TimeSpan OnEndingTurn(TimeSpan? forcedTime)
    {
        CurrentStopwatch.Stop();
        RemainingTime -= forcedTime ?? CurrentStopwatch.Elapsed;
        CurrentStopwatch.Reset();
        return CurrentStopwatch.Elapsed;
    }
}