using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures.Utilities;

internal static class FiguresHelper
{
    public static bool CanMoveTo(this ITile yourTile, ITile targetTile)
        => targetTile.IsEmpty();
    
    public static bool CanAttack(this ITile yourTile, ITile targetTile)
    {
        if (targetTile.Figure.Owner.Equals(yourTile.Figure.Owner) ||
            targetTile.Figure.Type is Wall)
        {
            return false;
        }
        
        if (!targetTile.Figure.Owner.Equals(Player.Neutral) ||
            !targetTile.IsEmpty())
        {
            return true;
        }

        return false;
    }
}