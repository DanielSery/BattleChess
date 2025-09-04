namespace BattleChess3.Multiplayer.Scheduling;

internal class ScheduledTask<T> : IScheduledTask
{
    private Task<T>? _task;
    private readonly Func<Task<T>> _getTask;
    private readonly TaskCompletionSource<T> _taskCompletionSource;

    public ScheduledTask(Func<Task<T>> getTask)
    {
        _getTask = getTask;
        _taskCompletionSource = new TaskCompletionSource<T>();
    }

    public Task<T> GetTaskForWaiting()
    {
        return _taskCompletionSource.Task;
    }

    public Task GetExecutedTask()
    {
        var task = _getTask.Invoke();
        _task = task;
        return task;
    }

    public void SetResult()
    {
        _taskCompletionSource.SetResult(_task!.Result);
    }
}

public class ScheduledTask : IScheduledTask
{
    private readonly Func<Task> _getTask;
    private readonly TaskCompletionSource _taskCompletionSource;

    public ScheduledTask(Func<Task> getTask)
    {
        _getTask = getTask;
        _taskCompletionSource = new TaskCompletionSource();
    }

    public Task GetTaskForWaiting()
    {
        return _taskCompletionSource.Task;
    }
    
    public Task GetExecutedTask()
    {
        return _getTask.Invoke();
    }

    public void SetResult()
    {
        _taskCompletionSource.SetResult();
    }
}