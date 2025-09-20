using CrownsGuard.Core.Figures;
using CrownsGuard.FigureDefinitions.Localization;

namespace CrownsGuard.FigureDefinitions.Figures;

public class NeutralFigureTypeInfo : IFigureTypeInfo
{
    public NeutralFigureTypeInfo(string name, Figure figure)
    {
        Name = name;
        Figure = figure;
    }

    public string Name { get; }
    public Figure Figure { get; }
    public string DisplayName => CurrentLocalization.Instance[$"{Name}_{nameof(DisplayName)}"];
    public string BaseDescription => CurrentLocalization.Instance[$"{Name}_{nameof(BaseDescription)}"];
    public string MovementDescription => CurrentLocalization.Instance[$"{Name}_{nameof(MovementDescription)}"];
    public string AttackDescription => CurrentLocalization.Instance[$"{Name}_{nameof(AttackDescription)}"];
    public string SpecialDescription => CurrentLocalization.Instance[$"{Name}_{nameof(SpecialDescription)}"];

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{Name}0.png", UriKind.Absolute) },
        };
}