using System.Collections.Generic;
using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class KingTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Verify()
    {
        const int src = 27; // d4
        const Figure king = Figure.King | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ => { /* no neighbors */ },
            src,
            king,
            King.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_MixedBlockingAndAttacks_Verify()
    {
        const int src = 27; // d4
        const Figure king = Figure.King | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // Up-Left: enemy
                var ul = src.GetWithOffset(PositionConstants.UL);
                if (ul != -1) b[ul] = Figure.Peasant | Figure.IsBlack;

                // Up: empty
                var u = src.GetWithOffset(PositionConstants.U);
                if (u != -1) b[u] = Figure.Empty;

                // Up-Right: wall (non-walkable)
                var ur = src.GetWithOffset(PositionConstants.UR);
                if (ur != -1) b[ur] = Figure.Wall;

                // Left: ally
                var l = src.GetWithOffset(PositionConstants.L);
                if (l != -1) b[l] = Figure.Knight | Figure.IsWhite;

                // Right: enemy
                var r = src.GetWithOffset(PositionConstants.R);
                if (r != -1) b[r] = Figure.Archer | Figure.IsBlack;

                // Down-Left: empty
                var dl = src.GetWithOffset(PositionConstants.DL);
                if (dl != -1) b[dl] = Figure.Empty;

                // Down: ally
                var d = src.GetWithOffset(PositionConstants.D);
                if (d != -1) b[d] = Figure.LegionarySword | Figure.IsWhite;

                // Down-Right: wall
                var dr = src.GetWithOffset(PositionConstants.DR);
                if (dr != -1) b[dr] = Figure.Wall;
            },
            src,
            king,
            King.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_Castling_Black_Mixed_Verify()
    {
        // Black king at e1 with both rooks; right side clear, left side blocked
        const int src = 4; // e1
        const Figure king = Figure.King | Figure.IsBlack;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // Place allied corner pieces (any ally qualifies for castling in this variant)
                b[0] = Figure.MountedKnight | Figure.IsBlack; // a1
                b[7] = Figure.MountedKnight | Figure.IsBlack; // h1

                // Clear right side: f1, g1 empty
                b[5] = Figure.Empty;
                b[6] = Figure.Empty;

                // Block left side: at least one of b1,c1,d1 occupied
                b[3] = Figure.Knight | Figure.IsBlack; // d1 ally blocks
                // leave b1 and c1 empty to be explicit
                b[1] = Figure.Empty;
                b[2] = Figure.Empty;
            },
            src,
            king,
            King.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_Castling_White_Mixed_Verify()
    {
        // White king at e8 with both rooks; left side clear, right side blocked
        const int src = 60; // e8
        const Figure king = Figure.King | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // Place allied corner pieces (any ally qualifies)
                b[56] = Figure.MountedKnight | Figure.IsWhite; // a8
                b[63] = Figure.MountedKnight | Figure.IsWhite; // h8

                // Clear left side: b8, c8, d8 empty
                b[57] = Figure.Empty;
                b[58] = Figure.Empty;
                b[59] = Figure.Empty;

                // Block right side: at least one of f8,g8 occupied
                b[61] = Figure.Knight | Figure.IsWhite; // f8 ally blocks
                // ensure g8 empty
                b[62] = Figure.Empty;
            },
            src,
            king,
            King.GetPossibleActions
        ));
    }

    [Fact]
    public Task ExecuteCastle_Black_BothSides_Verify()
    {
        // Prepare two separate executions in one snapshot object
        var results = new List<object>();

        // Black king-side (e1->g1)
        {
            var board = TestUtils.EmptyBoard();
            board[4] = Figure.King | Figure.IsBlack;
            board[7] = Figure.MountedKnight | Figure.IsBlack;
            board[5] = Figure.Empty;
            board[6] = Figure.Empty;
            var action = new FigureAction(FigureActionType.Castling, 4, 6, board[4]);
            FigureActionExecutor.ExecuteFigureAction(board, action, TestUtils.IgnoreEvents());
            results.Add(new
            {
                scenario = "ExecuteCastle_Black_KingSide",
                nonEmpty = GetNonEmpty(board)
            });
        }

        // Black queen-side (e1->c1)
        {
            var board = TestUtils.EmptyBoard();
            board[4] = Figure.King | Figure.IsBlack;
            board[0] = Figure.MountedKnight | Figure.IsBlack;
            board[1] = Figure.Empty;
            board[2] = Figure.Empty;
            board[3] = Figure.Empty;
            var action = new FigureAction(FigureActionType.Castling, 4, 2, board[4]);
            FigureActionExecutor.ExecuteFigureAction(board, action, TestUtils.IgnoreEvents());
            results.Add(new
            {
                scenario = "ExecuteCastle_Black_QueenSide",
                nonEmpty = GetNonEmpty(board)
            });
        }

        return Verify(results);
    }

    [Fact]
    public Task ExecuteCastle_White_BothSides_Verify()
    {
        var results = new List<object>();

        // White queen-side (e8->c8)
        {
            var board = TestUtils.EmptyBoard();
            board[60] = Figure.King | Figure.IsWhite;
            board[56] = Figure.MountedKnight | Figure.IsWhite;
            board[57] = Figure.Empty;
            board[58] = Figure.Empty;
            board[59] = Figure.Empty;
            var action = new FigureAction(FigureActionType.Castling, 60, 58, board[60]);
            FigureActionExecutor.ExecuteFigureAction(board, action, TestUtils.IgnoreEvents());
            results.Add(new
            {
                scenario = "ExecuteCastle_White_QueenSide",
                nonEmpty = GetNonEmpty(board)
            });
        }

        // White king-side (e8->g8)
        {
            var board = TestUtils.EmptyBoard();
            board[60] = Figure.King | Figure.IsWhite;
            board[63] = Figure.MountedKnight | Figure.IsWhite;
            board[61] = Figure.Empty;
            board[62] = Figure.Empty;
            var action = new FigureAction(FigureActionType.Castling, 60, 62, board[60]);
            FigureActionExecutor.ExecuteFigureAction(board, action, TestUtils.IgnoreEvents());
            results.Add(new
            {
                scenario = "ExecuteCastle_White_KingSide",
                nonEmpty = GetNonEmpty(board)
            });
        }

        return Verify(results);
    }

    private static string[] GetNonEmpty(Figure[] board)
    {
        var list = new List<string>();
        for (int i = 0; i < 64; i++)
        {
            if (board[i] != Figure.Empty)
                list.Add($"({i/8},{i%8}):{board[i]}");
        }
        return list.ToArray();
    }
}
