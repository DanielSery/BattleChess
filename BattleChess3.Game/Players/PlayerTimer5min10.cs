// Copyright (c) Veeam Software Group GmbH

using System.Diagnostics;

namespace BattleChess3.Game.Players;

public class PlayerTimer5min10 : IPlayerTimer
{
    private readonly Stopwatch _currentStopwatch = new();
    public TimeSpan RemainingTime { get; private set; } = TimeSpan.FromMinutes(5);

    public void StartTurnTimer()
    {
        RemainingTime += TimeSpan.FromSeconds(10);
        _currentStopwatch.Start();
    }

    public TimeSpan EndTurnTimer(TimeSpan? forcedTime)
    {
        _currentStopwatch.Stop();
        var turnDuration = forcedTime ?? _currentStopwatch.Elapsed;
        RemainingTime -= turnDuration;
        _currentStopwatch.Reset();
        return turnDuration;
    }
}