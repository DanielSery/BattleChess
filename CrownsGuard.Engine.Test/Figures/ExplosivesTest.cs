using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class ExplosivesTest
{
    [Fact]
    public Task OnAttacked_CenterPosition_DestroysAllAdjacent()
    {
        const int src = 27; // d4 - center of board for easy adjacent access
        var board = new Figure[64];
        board[src] = Figure.Explosives | Figure.IsWhite;

        // Place various figures around the explosives
        var ul = src.GetWithOffset(PositionConstants.U1L1); if (ul != -1) board[ul] = Figure.Peasant | Figure.IsBlack;
        var u = src.GetWithOffset(PositionConstants.U1); if (u != -1) board[u] = Figure.Archer | Figure.IsWhite;
        var ur = src.GetWithOffset(PositionConstants.U1R1); if (ur != -1) board[ur] = Figure.Knight | Figure.IsBlack;
        var l = src.GetWithOffset(PositionConstants.L1); if (l != -1) board[l] = Figure.Mage | Figure.IsWhite;
        var r = src.GetWithOffset(PositionConstants.R1); if (r != -1) board[r] = Figure.Wizzard | Figure.IsBlack;
        var dl = src.GetWithOffset(PositionConstants.D1L1); if (dl != -1) board[dl] = Figure.Builder | Figure.IsWhite;
        var d = src.GetWithOffset(PositionConstants.D1); if (d != -1) board[d] = Figure.Barbarian | Figure.IsBlack;
        var dr = src.GetWithOffset(PositionConstants.D1R1); if (dr != -1) board[dr] = Figure.Dragon | Figure.IsWhite;

        var events = new List<BoardEvent>();
        Explosives.OnAttacked(new BoardEvent(BoardEventType.Attacked, 99, src), board, (e, _) => events.Add(e));

        return Verify(new
        {
            Board = board.Select(f => f.ToString()).ToArray(),
            Events = events
        });
    }

    [Fact]
    public Task OnAttacked_EdgePosition_OnlyValidPositionsDestroyed()
    {
        const int src = 0; // a1 - corner position
        var board = new Figure[64];
        board[src] = Figure.Explosives | Figure.IsBlack;

        // Place figures in positions that should be destroyed (only 3 positions from corner)
        var ur = src.GetWithOffset(PositionConstants.U1R1); if (ur != -1) board[ur] = Figure.Peasant | Figure.IsWhite;
        var r = src.GetWithOffset(PositionConstants.R1); if (r != -1) board[r] = Figure.Archer | Figure.IsBlack;
        var dr = src.GetWithOffset(PositionConstants.D1R1); if (dr != -1) board[dr] = Figure.Knight | Figure.IsWhite;

        var events = new List<BoardEvent>();
        Explosives.OnAttacked(new BoardEvent(BoardEventType.Attacked, 99, src), board, (e, _) => events.Add(e));

        return Verify(new
        {
            Board = board.Select(f => f.ToString()).ToArray(),
            Events = events
        });
    }

    [Fact]
    public Task OnAttacked_SideEdge_MixedValidInvalidPositions()
    {
        const int src = 4; // a5 - left edge, middle row
        var board = new Figure[64];
        board[src] = Figure.Explosives | Figure.IsWhite;

        // Place figures in positions that should be destroyed (5 positions from left edge)
        var ul = src.GetWithOffset(PositionConstants.U1L1); if (ul != -1) board[ul] = Figure.Peasant | Figure.IsBlack;
        var u = src.GetWithOffset(PositionConstants.U1); if (u != -1) board[u] = Figure.Archer | Figure.IsWhite;
        var ur = src.GetWithOffset(PositionConstants.U1R1); if (ur != -1) board[ur] = Figure.Knight | Figure.IsBlack;
        var r = src.GetWithOffset(PositionConstants.R1); if (r != -1) board[r] = Figure.Mage | Figure.IsWhite;
        var dl = src.GetWithOffset(PositionConstants.D1L1); if (dl != -1) board[dl] = Figure.Builder | Figure.IsBlack;
        var d = src.GetWithOffset(PositionConstants.D1); if (d != -1) board[d] = Figure.Barbarian | Figure.IsWhite;
        var dr = src.GetWithOffset(PositionConstants.D1R1); if (dr != -1) board[dr] = Figure.Dragon | Figure.IsBlack;

        var events = new List<BoardEvent>();
        Explosives.OnAttacked(new BoardEvent(BoardEventType.Attacked, 99, src), board, (e, _) => events.Add(e));

        return Verify(new
        {
            Board = board.Select(f => f.ToString()).ToArray(),
            Events = events
        });
    }

    [Fact]
    public Task OnAttacked_WithWallsAndEmptySpaces()
    {
        const int src = 27; // d4
        var board = new Figure[64];
        board[src] = Figure.Explosives | Figure.IsBlack;

        // Mix of walls, empty spaces, and figures
        var ul = src.GetWithOffset(PositionConstants.U1L1); if (ul != -1) board[ul] = Figure.Wall;
        var u = src.GetWithOffset(PositionConstants.U1); if (u != -1) board[u] = Figure.Empty; // Empty should be destroyed
        var ur = src.GetWithOffset(PositionConstants.U1R1); if (ur != -1) board[ur] = Figure.Peasant | Figure.IsWhite;
        var l = src.GetWithOffset(PositionConstants.L1); if (l != -1) board[l] = Figure.Empty; // Empty should be destroyed
        var r = src.GetWithOffset(PositionConstants.R1); if (r != -1) board[r] = Figure.Wall;
        var dl = src.GetWithOffset(PositionConstants.D1L1); if (dl != -1) board[dl] = Figure.Archer | Figure.IsBlack;
        var d = src.GetWithOffset(PositionConstants.D1); if (d != -1) board[d] = Figure.Empty; // Empty should be destroyed
        var dr = src.GetWithOffset(PositionConstants.D1R1); if (dr != -1) board[dr] = Figure.Wall;

        var events = new List<BoardEvent>();
        Explosives.OnAttacked(new BoardEvent(BoardEventType.Attacked, 99, src), board, (e, _) => events.Add(e));

        return Verify(new
        {
            Board = board.Select(f => f.ToString()).ToArray(),
            Events = events
        });
    }
}