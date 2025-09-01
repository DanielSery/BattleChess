using BattleChess3.Core.Figures;

namespace BattleChess3.Maps.Figures;

public interface IFigureCreator
{
    Figure CreateFigure(FigureBlueprint figureBlueprint);

    Figure CreateEmptyFigure();
}