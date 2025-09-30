using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class MinerTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Verify()
    {
        const int src = 27; // d4
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ =>
            {
                /* empty board - all positions are walkable except edges */
            },
            src,
            miner,
            Miner.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_WithObstacles_Verify()
    {
        const int src = 27; // d4
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // Left: empty then friendly blocker at distance 2
                var l1 = src.GetWithOffset(PositionConstants.L);
                var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L);
                if (l1 != -1) b[l1] = Figure.Empty; // walkable
                if (l2 != -1) b[l2] = Figure.Peasant | Figure.IsWhite; // friendly blocks

                // Up: empty spaces for miner movement
                var u1 = src.GetWithOffset(PositionConstants.U);
                var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U);
                if (u1 != -1) b[u1] = Figure.Empty; // walkable
                if (u2 != -1) b[u2] = Figure.Empty; // walkable
                if (u3 != -1) b[u3] = Figure.Empty; // walkable

                // Right: wall immediately
                var r1 = src.GetWithOffset(PositionConstants.R);
                if (r1 != -1) b[r1] = Figure.Wall; // non-walkable blocker

                // Down: friendly immediately
                var d1 = src.GetWithOffset(PositionConstants.D);
                if (d1 != -1) b[d1] = Figure.LegionarySword | Figure.IsWhite; // non-walkable blocker
            },
            src,
            miner,
            Miner.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_Verify()
    {
        const int src = 0; // a1
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ =>
            {
                /* empty board - only right and up directions available */
            },
            src,
            miner,
            Miner.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_LongDistanceMoves_Verify()
    {
        const int src = 27; // d4
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // Set up a clear path in all directions for multiple squares
                // Left direction: positions 26, 25, 24 should be empty
                var l1 = src.GetWithOffset(PositionConstants.L);
                var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L);
                var l3 = l2 == -1 ? -1 : l2.GetWithOffset(PositionConstants.L);
                if (l1 != -1) b[l1] = Figure.Empty;
                if (l2 != -1) b[l2] = Figure.Empty;
                if (l3 != -1) b[l3] = Figure.Empty;

                // Right direction: positions 28, 29, 30 should be empty
                var r1 = src.GetWithOffset(PositionConstants.R);
                var r2 = r1 == -1 ? -1 : r1.GetWithOffset(PositionConstants.R);
                var r3 = r2 == -1 ? -1 : r2.GetWithOffset(PositionConstants.R);
                if (r1 != -1) b[r1] = Figure.Empty;
                if (r2 != -1) b[r2] = Figure.Empty;
                if (r3 != -1) b[r3] = Figure.Empty;

                // Up direction: positions 19, 11, 3 should be empty
                var u1 = src.GetWithOffset(PositionConstants.U);
                var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U);
                if (u1 != -1) b[u1] = Figure.Empty;
                if (u2 != -1) b[u2] = Figure.Empty;
                if (u3 != -1) b[u3] = Figure.Empty;

                // Down direction: positions 35, 43, 51 should be empty
                var d1 = src.GetWithOffset(PositionConstants.D);
                var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D);
                var d3 = d2 == -1 ? -1 : d2.GetWithOffset(PositionConstants.D);
                if (d1 != -1) b[d1] = Figure.Empty;
                if (d2 != -1) b[d2] = Figure.Empty;
                if (d3 != -1) b[d3] = Figure.Empty;
            },
            src,
            miner,
            Miner.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_FireWalkable_Verify()
    {
        const int src = 27; // d4
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // Place Fire squares in all directions - these should be walkable for miner
                var l1 = src.GetWithOffset(PositionConstants.L);
                var r1 = src.GetWithOffset(PositionConstants.R);
                var u1 = src.GetWithOffset(PositionConstants.U);
                var d1 = src.GetWithOffset(PositionConstants.D);

                if (l1 != -1) b[l1] = Figure.Fire;
                if (r1 != -1) b[r1] = Figure.Fire;
                if (u1 != -1) b[u1] = Figure.Fire;
                if (d1 != -1) b[d1] = Figure.Fire;

                // Place Fire squares further along to test MinerMove through Fire
                var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L);
                var r2 = r1 == -1 ? -1 : r1.GetWithOffset(PositionConstants.R);
                var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D);

                if (l2 != -1) b[l2] = Figure.Fire;
                if (r2 != -1) b[r2] = Figure.Fire;
                if (u2 != -1) b[u2] = Figure.Fire;
                if (d2 != -1) b[d2] = Figure.Fire;
            },
            src,
            miner,
            Miner.GetPossibleActions
        ));
    }

    [Fact]
    public Task ExecuteMove_ShortMoveNoTrench_Verify()
    {
        const int src = 27; // d4
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(_ =>
            {
                /* empty board */
            },
            src,
            miner,
            PositionConstants.U, // Move up one square
            FigureActionType.MinerMove));
    }

    [Fact]
    public Task ExecuteMove_LongMoveMultipleTrenches_Verify()
    {
        const int src = 27; // d4
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(b =>
            {
                // Ensure path is clear for 3 squares up: positions 27->19->11->3
                var u1 = src.GetWithOffset(PositionConstants.U);
                var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U);

                if (u1 != -1) b[u1] = Figure.Empty;
                if (u2 != -1) b[u2] = Figure.Empty;
                if (u3 != -1) b[u3] = Figure.Empty;
            },
            src,
            miner,
            PositionConstants.U * 3, // Move up three squares
            FigureActionType.MinerMove));
    }

    [Fact]
    public Task ExecuteMove_WithObstacleStopsTrench_Verify()
    {
        const int src = 27; // d4
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(b =>
            {
                // Place obstacle after 2 squares up, path: 27->19->11 (obstacle)->3 (should not create trench)
                var u1 = src.GetWithOffset(PositionConstants.U);
                var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U);

                if (u1 != -1) b[u1] = Figure.Empty;
                if (u2 != -1) b[u2] = Figure.Empty;
                if (u3 != -1) b[u3] = Figure.Wall; // obstacle stops trench creation
            },
            src,
            miner,
            PositionConstants.U * 2, // Move up two squares
            FigureActionType.MinerMove));
    }

    [Fact]
    public Task ExecuteMove_BoundaryNoTrenchOffBoard_Verify()
    {
        const int src = 4; // b8 (top edge, but can move right)
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(_ =>
            {
                /* empty board */
            },
            src,
            miner,
            PositionConstants.R, // Move right one square (to edge)
            FigureActionType.MinerMove));
    }

    [Fact]
    public Task ExecuteMove_DirectionRight_Verify()
    {
        const int src = 27; // d4
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(_ =>
            {
                /* empty board */
            },
            src,
            miner,
            PositionConstants.R, // Move right one square
            FigureActionType.MinerMove));
    }

    [Fact]
    public Task ExecuteMove_DirectionLeft_Verify()
    {
        const int src = 27; // d4
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(_ =>
            {
                /* empty board */
            },
            src,
            miner,
            PositionConstants.L, // Move left one square
            FigureActionType.MinerMove));
    }

    [Fact]
    public Task ExecuteMove_DirectionUp_Verify()
    {
        const int src = 27; // d4
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(_ =>
            {
                /* empty board */
            },
            src,
            miner,
            PositionConstants.U, // Move up one square
            FigureActionType.MinerMove));
    }

    [Fact]
    public Task ExecuteMove_DirectionDown_Verify()
    {
        const int src = 27; // d4
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(_ =>
            {
                /* empty board */
            },
            src,
            miner,
            PositionConstants.D, // Move down one square
            FigureActionType.MinerMove));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_H1_Verify()
    {
        const int src = 7; // h1 (bottom-right corner)
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ =>
            {
                /* empty board - only left and up directions available */
            },
            src,
            miner,
            Miner.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A8_Verify()
    {
        const int src = 56; // a8 (top-left corner)
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ =>
            {
                /* empty board - only right and down directions available */
            },
            src,
            miner,
            Miner.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_H8_Verify()
    {
        const int src = 63; // h8 (top-right corner)
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ =>
            {
                /* empty board - no movement possible */
            },
            src,
            miner,
            Miner.GetPossibleActions
        ));
    }

    [Fact]
    public Task ExecuteMove_EdgeCase_CornerToCorner_Verify()
    {
        const int src = 0; // a1 (bottom-left corner)
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(_ =>
            {
                // Ensure path is clear for diagonal movement to h8 (position 63)
                // This should create trenches along the diagonal path
            },
            src,
            miner,
            PositionConstants.UR * 7, // Move diagonally to h8 (7 right, 7 up)
            FigureActionType.MinerMove));
    }

    [Fact]
    public Task ExecuteMove_EdgeCase_AlongEdge_Verify()
    {
        const int src = 0; // a1 (bottom-left corner)
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(b =>
            {
                // Ensure path is clear along the bottom edge: positions 0->1->2->3->4->5->6->7
                for (var i = 1; i < 8; i++) b[i] = Figure.Empty;
            },
            src,
            miner,
            PositionConstants.R * 7, // Move right along bottom edge to h1
            FigureActionType.MinerMove));
    }

    [Fact]
    public Task ExecuteMove_EdgeCase_BoundaryTrenchCreation_Verify()
    {
        const int src = 1; // b1 (near bottom edge)
        const Figure miner = Figure.Miner | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(b =>
            {
                // Set up clear path upward but close to right edge
                // Move up 6 squares: positions 1->9->17->25->33->41->49
                for (var i = 1; i < 7; i++)
                {
                    var pos = src.GetWithOffset((short)(PositionConstants.U * i));
                    if (pos != -1) b[pos] = Figure.Empty;
                }
            },
            src,
            miner,
            PositionConstants.U * 6, // Move up 6 squares toward top edge
            FigureActionType.MinerMove));
    }
}