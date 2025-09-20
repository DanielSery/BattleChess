namespace CrownsGuard.Game.Players;

public interface IAutomaticallyControlledPlayer : IPlayer
{
    public Task HandleAutomaticTurnAsync();
}