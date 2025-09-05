namespace CrownsGuard.Multiplayer.Scheduling;

public interface IMultiplayerScheduler
{
    Lock SyncLock { get; }

    Task<T> QueueTask<T>(Func<Task<T>> getTask);

    Task QueueTask(Func<Task> getTask);

    void WaitForFinish();
}