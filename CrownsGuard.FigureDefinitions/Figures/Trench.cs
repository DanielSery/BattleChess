using CrownsGuard.Core.Figures;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Trench : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Trench;

    public IDictionary<int, Uri> ImageUris =>
        new Dictionary<int, Uri>
        {
            { 0, new Uri($"pack://application:,,,/CrownsGuard.FigureDefinitions;component/Images/{GetType().Name}.png", UriKind.Absolute) },
        };
}