using BattleChess3.Game.Board;

namespace BattleChess3.Maps;

public interface IMapLoader
{
    void LoadMap(IBoard board, MapBlueprint map);
    void CreateFigure(ITile tile, FigureBlueprint figureBlueprint);
}