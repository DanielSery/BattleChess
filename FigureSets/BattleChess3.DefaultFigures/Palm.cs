using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.DefaultFigures;

public class Palm : IDefaultFigureType
{
    int IFigureType.FigureId => 1;
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return Array.Empty<FigureAction>();
    }
}