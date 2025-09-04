namespace BattleChess3.Multiplayer.Scheduling;

public interface IScheduledTask
{
    Task GetExecutedTask();
    void SetResult();
}