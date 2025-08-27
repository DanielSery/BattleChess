using System.Diagnostics;
using BattleChess3.Game.Figures;

namespace BattleChess3.Game.Players;

public class PlayerInfo
{
    public static readonly PlayerInfo Neutral = new(Player.Neutral, "Neutral", null, null);

    public PlayerInfo(Player player, string playerName, string? playerId, int? elo)
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

    public void StartTurn()
    {
        CurrentStopwatch.Start();
    }

    public TimeSpan OnEndingTurn(TimeSpan? forcedTime)
    {
        CurrentStopwatch.Stop();
        var turnDuration = forcedTime ?? CurrentStopwatch.Elapsed;
        RemainingTime -= turnDuration;
        CurrentStopwatch.Reset();
        return turnDuration;
    }

    public void AddTime(TimeSpan timeSpan)
    {
        RemainingTime += timeSpan;
    }

    public override string ToString()
    {
        return Name;
    }
}