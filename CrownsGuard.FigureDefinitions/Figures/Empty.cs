using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Localization;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Empty : IFigureType
{
    public int FigureValue => 0;

    public int FigureId => (int)CrownsGuardFigureIds.EmptyId;
    
    public string DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.DisplayName)}"];
    public string BaseDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.BaseDescription)}"];
    public string MovementDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.MovementDescription)}"];
    public string AttackDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.AttackDescription)}"];
    public string SpecialDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureType.SpecialDescription)}"];

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