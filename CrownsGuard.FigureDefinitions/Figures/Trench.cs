using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Trench : ICrownsGuardFigureType
{
    public int FigureValue => 0;

    public int FigureId => (int)CrownsGuardFigureIds.TrenchId;
    
    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return [];
    }
}