using BattleChess3.Core.Figures;

namespace BattleChess3.Maps.Figures;

public interface IFigureCreator
{
    IFigure CreateFigure(FigureBlueprint figureBlueprint);

    IFigure CreateEmptyFigure();
}