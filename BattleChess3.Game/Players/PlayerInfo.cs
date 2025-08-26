using System.Diagnostics;
using BattleChess3.Game.Figures;

namespace BattleChess3.Game.Players;

public class PlayerInfo
{
    public static readonly PlayerInfo Neutral = new(null, "Neutral", 0, 0);

    public PlayerInfo(string? playerId, string playerName, int? elo, Player player)
    {
        Player = player;
        PlayerId = playerId;
        Name = playerName;
        Elo = elo;
        CurrentStopwatch = new Stopwatch();
        RemainingTime = TimeSpan.FromMinutes(5);
    }

    public Player Player { get; }
    public string? PlayerId { get; }
    public string Name { get; }
    public int? Elo { get; }
    public List<Figure> Figures { get; } = [];
    public Stopwatch CurrentStopwatch { get; }
    public TimeSpan RemainingTime { get; private set; }

    public override string ToString()
    {
        return Name;
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