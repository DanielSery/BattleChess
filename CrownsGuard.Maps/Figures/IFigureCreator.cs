using CrownsGuard.Core.Figures;

namespace CrownsGuard.Maps.Figures;

public interface IFigureCreator
{
    IFigure CreateFigure(FigureBlueprint figureBlueprint);

    IFigure CreateEmptyFigure();
}