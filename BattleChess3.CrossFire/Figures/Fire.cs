using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.CrossFireFigures.Utilities;

namespace BattleChess3.CrossFireFigures.Figures;

public class Fire : ICrossFireFigureType
{
    public int FigureValue => 0;

    public int FigureId => CrossFireFigureIds.FireId;
    
    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/BattleChess3.CrossFireFigures;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return [];
    }
    
    public void OnDied(ITile unitTile, IBoard board)
    {
        if (unitTile.Figure.Type is Dragon)
            return;
        
        unitTile.Die(board);
    }
} 
