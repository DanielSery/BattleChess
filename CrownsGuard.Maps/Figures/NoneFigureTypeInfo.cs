using CrownsGuard.Core.Figures;

namespace CrownsGuard.Maps.Figures;

public class NoneFigureTypeInfo : IFigureTypeInfo
{
    public static NoneFigureTypeInfo Instance { get; } = new();

    /// <inheritdoc />
    public FigureId FigureId => FigureId.Empty;
    public int FigureValue => 0;
    public string DisplayName => string.Empty;
    public string BaseDescription => string.Empty;
    public string MovementDescription => string.Empty;
    public string AttackDescription => string.Empty;
    public string SpecialDescription => string.Empty;
    public IDictionary<int, Uri> ImageUris { get; } = new Dictionary<int, Uri>();
}