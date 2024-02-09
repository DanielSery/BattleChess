using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.StarWarsFigures;

public class Bomb : IStarWarsFigureType
{
    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        return FigureAction.None;
    }

    void IFigureType.OnDied(ITile unitTile, IBoard board)
    {
        unitTile.Die(board);
    }
}