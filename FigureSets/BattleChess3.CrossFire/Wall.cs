using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Wall : ICrossFireFigureType
{
    int IFigureType.FigureId => 7;
    
    IDictionary<int, Uri> IFigureType.ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/BattleChess3.CrossFireFigures;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return [];
    }
}