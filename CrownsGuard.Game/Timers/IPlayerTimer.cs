namespace CrownsGuard.Game.Timers;

public interface IPlayerTimer
{
    TimeSpan RemainingTime { get; }
    
    TimeSpan LastTurnElapsedTime { get; }
    
    void StartTurnTimer();

    void EndTurnTimer(TimeSpan? forcedTime);
}