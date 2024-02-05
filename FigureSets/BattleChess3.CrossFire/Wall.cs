using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.CrossFireFigures;

public class Wall : ICrossFireFigureType
{
    public static readonly ICrossFireFigureType Instance = new Wall();

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        return FigureAction.None;
    }
}
