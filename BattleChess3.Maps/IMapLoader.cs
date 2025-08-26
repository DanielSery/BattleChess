using BattleChess3.Game.GameBoard;

namespace BattleChess3.Maps;

public interface IMapLoader
{
    void LoadMap(IBoard board, MapBlueprint map);

    void LoadMapExtendedFor2Players(IBoard board, MapBlueprint map);
}