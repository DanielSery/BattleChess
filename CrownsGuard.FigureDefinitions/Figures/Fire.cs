using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Fire : ICrownsGuardFigureType
{
    public int FigureValue => 0;

    public int FigureId => (int)CrownsGuardFigureIds.FireId;
    
    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
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
