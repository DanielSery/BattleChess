using System.Collections.Concurrent;

namespace BattleChess3.Multiplayer.Scheduling;

internal class MultiplayerScheduler : IMultiplayerScheduler
{
    private Task? _runningTask;
    private readonly ConcurrentQueue<IScheduledTask> _queuedTasks = new ConcurrentQueue<IScheduledTask>();
    private int _currentThreadId;

    public Lock SyncLock { get; } = new Lock();

    /// <inheritdoc />
    public Task<T> QueueTask<T>(Func<Task<T>> getTask)
    {
        if (_currentThreadId == Environment.CurrentManagedThreadId)
        {
            try
            {
                return getTask.Invoke();     
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

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
        if (_currentThreadId == Environment.CurrentManagedThreadId)
        {
            try
            {
                return getTask.Invoke();     
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

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
        _currentThreadId = Environment.CurrentManagedThreadId;
        while (TryGetTaskToRun(out var currentScheduledTask))
        {
            try
            {
                await currentScheduledTask!.GetExecutedTask();
                currentScheduledTask.SetResult();            
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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