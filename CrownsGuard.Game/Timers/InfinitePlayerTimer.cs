namespace CrownsGuard.Game.Timers;

public class InfinitePlayerTimer : IPlayerTimer
{
    public static readonly InfinitePlayerTimer Instance = new();

    private InfinitePlayerTimer() { }

    /// <inheritdoc />
    public TimeSpan RemainingTime => TimeSpan.MaxValue;

    /// <inheritdoc />
    public TimeSpan LastTurnElapsedTime => TimeSpan.Zero;

    /// <inheritdoc />
    public void StartTurnTimer()
    {
    }

    /// <inheritdoc />
    public void EndTurnTimer(TimeSpan? forcedTime)
    {
    }
}