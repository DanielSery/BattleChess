using BattleChess3.Core.Players;

namespace BattleChess3.Core;

public interface IFigureOwnersHolder
{
    /// <summary>
    ///     Gets player with id.
    /// </summary>
    IFigureOwner GetFigureOwner(Player player);
}