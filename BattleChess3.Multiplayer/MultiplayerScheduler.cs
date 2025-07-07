using System.Collections.Concurrent;

namespace BattleChess3.Multiplayer;

internal class MultiplayerScheduler : IMultiplayerScheduler
{
    private Task? _runningTask;
    private readonly ConcurrentQueue<IScheduledTask> _queuedTasks = new ConcurrentQueue<IScheduledTask>();
    
    public object SyncLock { get; } = new object();

    /// <inheritdoc />
    public Task<T> QueueTask<T>(Func<Task<T>> getTask)
    {
        lock (SyncLock)
        {
            var scheduledTask = new ScheduledTask<T>(getTask);
            _queuedTasks.Enqueue(scheduledTask);
            _runningTask ??= Task.Run(ExecuteQueue);
            return scheduledTask.GetTaskForWaiting();
        }
    }

    public Task QueueTask(Func<Task> getTask)
    {
        lock (SyncLock)
        {
            var scheduledTask = new ScheduledTask(getTask);
            _queuedTasks.Enqueue(scheduledTask);
            _runningTask ??= Task.Run(ExecuteQueue);
            return scheduledTask.GetTaskForWaiting();
        }
    }

    private async Task ExecuteQueue()
    {
        while (TryGetTaskToRun(out var currentScheduledTask))
        {
            await currentScheduledTask!.GetExecutedTask();
            currentScheduledTask.SetResult();
        }
    }

    private bool TryGetTaskToRun(out IScheduledTask? scheduledTask)
    {
        lock (SyncLock)
        {
            if (_queuedTasks.TryDequeue(out var getTask))
            {
                scheduledTask = getTask;
                return true;
            }

            scheduledTask = null;
            _runningTask = null;
            return false;
        }
    }

    public void WaitForFinish()
    {
        _runningTask?.Wait();
    }
}