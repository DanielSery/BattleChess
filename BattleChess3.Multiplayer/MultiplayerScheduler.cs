using System.Collections.Concurrent;

namespace BattleChess3.Multiplayer;

internal class MultiplayerScheduler : IMultiplayerScheduler
{
    private Task? _runningTask;
    private readonly ConcurrentQueue<Func<Task>> _queuedTasks = new ConcurrentQueue<Func<Task>>();
    
    public object SyncLock { get; } = new object();

    public void QueueTask(Func<Task> getTask)
    {
        lock (SyncLock)
        {
            _queuedTasks.Enqueue(getTask);
            _runningTask ??= Task.Run(async () =>
            {
                while (TryGetTaskToRun(out var task))
                {
                    await task;
                }
            });
        }
    }

    private bool TryGetTaskToRun(out Task task)
    {
        lock (SyncLock)
        {
            if (_queuedTasks.TryDequeue(out var getTask))
            {
                task = getTask.Invoke();
                return true;
            }

            task = Task.CompletedTask;
            _runningTask = null;
            return false;
        }
    }

    public void WaitForFinish()
    {
        _runningTask?.Wait();
    }
}