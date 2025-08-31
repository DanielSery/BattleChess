using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;

namespace BattleChess3.CrossFireFigures.Figures;

public class Wall : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 1;
    
    int IFigureType.FigureId => CrossFireFigureIds.WallId;
    
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