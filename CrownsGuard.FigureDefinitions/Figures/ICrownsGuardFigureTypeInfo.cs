using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Localization;

namespace CrownsGuard.FigureDefinitions.Figures;

internal interface ICrownsGuardFigureTypeInfo : IFigureTypeInfo
{
    string IFigureTypeInfo.DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(DisplayName)}"];
    string IFigureTypeInfo.BaseDescription => string.Format(CurrentLocalization.Instance[$"{GetType().Name}_{nameof(BaseDescription)}"], FigureValue);
    string IFigureTypeInfo.MovementDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(MovementDescription)}"];
    string IFigureTypeInfo.AttackDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(AttackDescription)}"];
    string IFigureTypeInfo.SpecialDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(SpecialDescription)}"];

    IDictionary<int, Uri> IFigureTypeInfo.ImageUris =>
        new Dictionary<int, Uri>
        {
            { 1, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}1.png", UriKind.Absolute) },
            { 2, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}2.png", UriKind.Absolute) }
        };
}