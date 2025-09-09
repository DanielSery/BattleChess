using CrownsGuard.Core.Figures;

namespace CrownsGuard.Core.Players;

public interface IFigureOwner
{
    Player Player { get; }
    List<IFigure> Figures { get; }
}