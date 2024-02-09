using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.CrossFireFigures;

public class Wall : ICrossFireFigureType
{
    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        return FigureAction.None;
    }
}