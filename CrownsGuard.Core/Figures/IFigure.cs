using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.Figures;

public interface IFigure
{
    public Guid Id { get; }
    public IFigureOwner Owner { get; }
    public IFigureType Type { get; }
    public bool IsKing { get; }
}