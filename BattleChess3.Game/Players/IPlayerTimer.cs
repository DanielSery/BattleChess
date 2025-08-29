// Copyright (c) Veeam Software Group GmbH

namespace BattleChess3.Game.Players;

public interface IPlayerTimer
{
    TimeSpan RemainingTime { get; }
    
    void StartTurnTimer();

    TimeSpan EndTurnTimer(TimeSpan? forcedTime);
}