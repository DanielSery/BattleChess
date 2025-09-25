using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FiguresDesign.Localization;

namespace CrownsGuard.FiguresDesign.Figures;

public class PlayerFigureTypeInfo : IFigureTypeInfo
{
    public PlayerFigureTypeInfo(string name, Figure figure)
    {
        Name = name;
        Figure = figure;
    }

    public string Name { get; }
    public Figure Figure { get; }
    public string DisplayName => CurrentLocalization.Instance[$"{Name}_{nameof(DisplayName)}"];
    public string BaseDescription => string.Format(CurrentLocalization.Instance[$"{Name}_{nameof(BaseDescription)}"], Figure.GetFigureValue());
    public string MovementDescription => CurrentLocalization.Instance[$"{Name}_{nameof(MovementDescription)}"];
    public string AttackDescription => CurrentLocalization.Instance[$"{Name}_{nameof(AttackDescription)}"];
    public string SpecialDescription => CurrentLocalization.Instance[$"{Name}_{nameof(SpecialDescription)}"];

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 1, new Uri($"pack://application:,,,/CrownsGuard.FiguresDesign;component/Images/{Name}1.png", UriKind.Absolute) },
            { 2, new Uri($"pack://application:,,,/CrownsGuard.FiguresDesign;component/Images/{Name}2.png", UriKind.Absolute) }
        };
}