using CrownsGuard.Core.GameBoard;
using CrownsGuard.Maps.GameBoard;

namespace CrownsGuard.Maps.BoardBlueprints;

public interface IBoardLoader
{
    void LoadBoard(IBoardInfo boardInfo, BoardBlueprint map);

    void LoadTeamBoard(IBoardInfo boardInfo, BoardBlueprint map);

    void LoadBoardExtendedFor2Players(IBoardInfo boardInfo, BoardBlueprint map);
}