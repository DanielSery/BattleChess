// Copyright (c) Veeam Software Group GmbH

using System.Diagnostics;

namespace BattleChess3.Game.Players;

public class PlayerTimer5min10 : IPlayerTimer
{
    private readonly Stopwatch _currentStopwatch = new();
    public TimeSpan RemainingTime { get; private set; } = TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public TimeSpan LastTurnElapsedTime { get; private set; }

    public void StartTurnTimer()
    {
        RemainingTime += TimeSpan.FromSeconds(10);
        _currentStopwatch.Start();
    }

    public void EndTurnTimer(TimeSpan? forcedTime)
    {
        _currentStopwatch.Stop();
        LastTurnElapsedTime = forcedTime ?? _currentStopwatch.Elapsed;
        RemainingTime -= LastTurnElapsedTime;
        _currentStopwatch.Reset();
    }
}