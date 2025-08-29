// Copyright (c) Veeam Software Group GmbH

using System.Diagnostics;

namespace BattleChess3.Game.Timers;

public class ChessTimer : IPlayerTimer
{
    private readonly Stopwatch _currentStopwatch = new();
    private readonly TimeSpan _eachTurn;

    public ChessTimer(TimeSpan initial, TimeSpan eachTurn)
    {
        RemainingTime = initial;
        _eachTurn = eachTurn;
    }

    public TimeSpan RemainingTime { get; private set; }

    /// <inheritdoc />
    public TimeSpan LastTurnElapsedTime { get; private set; }

    public void StartTurnTimer()
    {
        RemainingTime += _eachTurn;
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