namespace BattleChess3.Multiplayer;

public interface IMultiplayerScheduler
{
    Lock SyncLock { get; }

    Task<T> QueueTask<T>(Func<Task<T>> getTask);

    Task QueueTask(Func<Task> getTask);

    void WaitForFinish();
}