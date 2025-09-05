namespace CrownsGuard.Multiplayer.Scheduling;

public interface IScheduledTask
{
    Task GetExecutedTask();
    void SetResult();
}