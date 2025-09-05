// Copyright (c) Veeam Software Group GmbH

namespace CrownsGuard.Game.Timers;

public interface IPlayerTimer
{
    TimeSpan RemainingTime { get; }
    
    TimeSpan LastTurnElapsedTime { get; }
    
    void StartTurnTimer();

    void EndTurnTimer(TimeSpan? forcedTime);
}