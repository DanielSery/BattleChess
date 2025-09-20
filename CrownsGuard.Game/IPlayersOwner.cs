
using CrownsGuard.Game.Players;

namespace CrownsGuard.Game;

public interface IPlayersOwner
{
    /// <summary>
    ///     Gets player with id.
    /// </summary>
    IPlayer GetPlayer(PlayerColor playerColor);
}