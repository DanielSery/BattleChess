namespace BattleChess3.Multiplayer;

public interface IMultiplayerScheduler
{
    object SyncLock { get; }

    void QueueTask(Func<Task> getTask);

    void WaitForFinish();
}