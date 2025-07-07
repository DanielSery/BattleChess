namespace BattleChess3.Multiplayer;

public interface IScheduledTask
{
    Task GetExecutedTask();
    void SetResult();
}