using CrownsGuard.Game.Players;

namespace CrownsGuard.Multiplayer.Players;

public interface IAutomaticallyControlledPlayerInfo : IPlayerInfo
{
    public Task HandleAutomaticTurnAsync();
}