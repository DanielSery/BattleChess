using BattleChess3.Core.Figures;

namespace BattleChess3.Maps;

public interface IFigureCreator
{
    Figure CreateFigure(FigureBlueprint figureBlueprint);

    Figure CreateEmptyFigure();
}