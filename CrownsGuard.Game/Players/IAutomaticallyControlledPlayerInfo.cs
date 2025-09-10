namespace CrownsGuard.Game.Players;

public interface IAutomaticallyControlledPlayerInfo : IPlayerInfo
{
    public Task HandleAutomaticTurnAsync();
}