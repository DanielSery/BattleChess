namespace CrownsGuard.Core.Figures;

public class NoneFigureType : IFigureType
{
    public static NoneFigureType Instance { get; } = new();

    public int FigureValue => 0;
    public string DisplayName => string.Empty;
    public string BaseDescription => string.Empty;
    public string MovementDescription => string.Empty;
    public string AttackDescription => string.Empty;
    public string SpecialDescription => string.Empty;
    public IDictionary<int, Uri> ImageUris { get; } = new Dictionary<int, Uri>();
}