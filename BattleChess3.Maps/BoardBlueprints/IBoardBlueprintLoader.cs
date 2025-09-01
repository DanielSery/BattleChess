using BattleChess3.Core.GameBoard;

namespace BattleChess3.Maps.BoardBlueprints;

public interface IBoardBlueprintLoader
{
    void LoadMap(IBoard board, BoardBlueprint map);

    void LoadMapExtendedFor2Players(IBoard board, BoardBlueprint map);
}