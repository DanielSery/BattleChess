using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;

namespace BattleChess3.CrossFireFigures.Figures;

public class Trench : ICrossFireFigureType
{
    public int FigureValue => 0;

    public int FigureId => CrossFireFigureIds.TrenchId;
    
    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/BattleChess3.CrossFireFigures;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return [];
    }
}