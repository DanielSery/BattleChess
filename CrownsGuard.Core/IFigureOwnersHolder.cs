using CrownsGuard.Core.Players;

namespace CrownsGuard.Core;

public interface IFigureOwnersHolder
{
    /// <summary>
    ///     Gets player with id.
    /// </summary>
    IFigureOwner GetFigureOwner(Player player);
}