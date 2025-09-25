using CrownsGuard.FiguresDesign.Figures;
using CrownsGuard.Game.Players;

namespace CrownsGuard.Maps.Figures;

public interface IFigureWithInfo : IFigureInfo
{
    public IPlayer Owner { get; }
    public IFigureTypeInfo TypeInfo { get; }
    public bool IsKing { get; }
}