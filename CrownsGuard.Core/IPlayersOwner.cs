using CrownsGuard.Core.Players;

namespace CrownsGuard.Core;

public interface IPlayersOwner
{
    /// <summary>
    ///     Gets player with id.
    /// </summary>
    IPlayer GetPlayer(PlayerColor playerColor);
}