using BattleChess3.Game.GameBoard;

namespace BattleChess3.Maps;

public interface IMapLoader
{
    void LoadMap(IBoard board, BoardBlueprint map);

    void LoadMapExtendedFor2Players(IBoard board, BoardBlueprint map);
}