using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.StarWarsFigures;

public class Bomb : IStarWarsFigureType
{
    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        return FigureAction.None;
    }

    void IFigureType.OnDied(ITile unitTile, ITile[] board)
    {
        unitTile.Die(board);
    }
}