// Copyright (c) Veeam Software Group GmbH

namespace BattleChess3.Game.Timers;

public interface IPlayerTimer
{
    TimeSpan RemainingTime { get; }
    
    TimeSpan LastTurnElapsedTime { get; }
    
    void StartTurnTimer();

    void EndTurnTimer(TimeSpan? forcedTime);
}