using BattleChess3.Game.Figures;

namespace BattleChess3.Maps;

public interface IFigureCreator
{
    Figure CreateFigure(FigureIdentifier figureIdentifier);

    Figure CreateEmptyFigure();
}