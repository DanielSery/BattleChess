using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.CrossFireFigures;

public class Wall : ICrossFireFigureType
{
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return Array.Empty<FigureAction>();
    }
}