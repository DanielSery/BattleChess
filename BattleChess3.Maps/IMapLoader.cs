using BattleChess3.Game.Board;

namespace BattleChess3.Maps;

public interface IMapLoader
{
    void LoadTeamMap(IBoard board, MapBlueprint map);

    void Load2PlayerMap(IBoard board, MapBlueprint map);
}