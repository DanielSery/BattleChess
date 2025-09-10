using CrownsGuard.Core.Figures;
using CrownsGuard.Core.SimulatedBoard;

namespace CrownsGuard.Maps.Figures;

public interface IFigureCreator
{
    IFigure CreateFigure(Figure figureBlueprint);

    IFigure CreateEmptyFigure();
}