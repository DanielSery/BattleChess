using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.Maps.BoardBlueprints;

public interface IBoardLoader
{
    void LoadBoard(IBoard board, BoardBlueprint map);

    void LoadTeamBoard(IBoard board, BoardBlueprint map);

    void LoadBoardExtendedFor2Players(IBoard board, BoardBlueprint map);
}