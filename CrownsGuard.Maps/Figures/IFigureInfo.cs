namespace CrownsGuard.Maps.Figures;

public interface IFigureInfo
{
    public string DisplayName { get; }
    public string BaseDescription { get; }
    public string MovementDescription { get; }
    public string AttackDescription { get; }
    public string SpecialDescription { get; }
    public Uri ImageUri { get; }
}
