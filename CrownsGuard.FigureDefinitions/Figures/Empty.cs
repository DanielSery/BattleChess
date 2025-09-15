using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Localization;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Empty : IFigureTypeInfo
{
    public int FigureValue => 0;
    public Figure Figure => Figure.Empty;
    
    public string DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureTypeInfo.DisplayName)}"];
    public string BaseDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureTypeInfo.BaseDescription)}"];
    public string MovementDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureTypeInfo.MovementDescription)}"];
    public string AttackDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureTypeInfo.AttackDescription)}"];
    public string SpecialDescription => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(IFigureTypeInfo.SpecialDescription)}"];

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
    }
}