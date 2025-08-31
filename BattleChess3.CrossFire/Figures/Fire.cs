using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.CrossFireFigures.Utilities;

namespace BattleChess3.CrossFireFigures.Figures;

public class Fire : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 0;
    
    int IFigureType.FigureId => CrossFireFigureIds.FireId;
    
    IDictionary<int, Uri> IFigureType.ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/BattleChess3.CrossFireFigures;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return [];
    }
    
    void IFigureType.OnDied(ITile unitTile, IBoard board)
    {
        if (unitTile.Figure.Type is Dragon)
            return;
        
        unitTile.Die(board);
    }
} 
