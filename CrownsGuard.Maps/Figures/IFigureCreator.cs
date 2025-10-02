using CrownsGuard.Core.Figures;

namespace CrownsGuard.Maps.Figures;

public interface IFigureCreator
{
    IFigureWithInfo CreateFigure(Figure figureBlueprint);
}