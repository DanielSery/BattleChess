using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class MinerTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        return Verify(new List<object>
        {
            // All empty scenario
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty board - all positions are walkable except edges */ },
                27, // d4
                Figure.Miner | Figure.IsWhite,
                Miner.GetPossibleActions),

            // With obstacles scenario
            TestUtils.RunGetActionsScenario(
                "With obstacles", b =>
                {
                    const int src = 27; // d4
                    // Left: empty then friendly blocker at distance 2
                    var l1 = src.GetWithOffset(PositionConstants.L1);
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L1);
                    if (l1 != -1) b[l1] = Figure.Empty; // walkable
                    if (l2 != -1) b[l2] = Figure.Peasant | Figure.IsWhite; // friendly blocks

                    // Up: empty spaces for miner movement
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Empty; // walkable
                    if (u2 != -1) b[u2] = Figure.Empty; // walkable
                    if (u3 != -1) b[u3] = Figure.Empty; // walkable

                    // Right: wall immediately
                    var r1 = src.GetWithOffset(PositionConstants.R1);
                    if (r1 != -1) b[r1] = Figure.Wall; // non-walkable blocker

                    // Down: friendly immediately
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    if (d1 != -1) b[d1] = Figure.LegionarySword | Figure.IsWhite; // non-walkable blocker
                },
                27, // d4
                Figure.Miner | Figure.IsWhite,
                Miner.GetPossibleActions),

            // Edge case A1 scenario
            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                _ => { /* empty board - only right and up directions available */ },
                0, // a1
                Figure.Miner | Figure.IsWhite,
                Miner.GetPossibleActions),

            // Long distance moves scenario
            TestUtils.RunGetActionsScenario(
                "Long distance moves", b =>
                {
                    const int src = 27; // d4
                    // Set up a clear path in all directions for multiple squares
                    // Left direction: positions 26, 25, 24 should be empty
                    var l1 = src.GetWithOffset(PositionConstants.L1);
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L1);
                    var l3 = l2 == -1 ? -1 : l2.GetWithOffset(PositionConstants.L1);
                    if (l1 != -1) b[l1] = Figure.Empty;
                    if (l2 != -1) b[l2] = Figure.Empty;
                    if (l3 != -1) b[l3] = Figure.Empty;

                    // Right direction: positions 28, 29, 30 should be empty
                    var r1 = src.GetWithOffset(PositionConstants.R1);
                    var r2 = r1 == -1 ? -1 : r1.GetWithOffset(PositionConstants.R1);
                    var r3 = r2 == -1 ? -1 : r2.GetWithOffset(PositionConstants.R1);
                    if (r1 != -1) b[r1] = Figure.Empty;
                    if (r2 != -1) b[r2] = Figure.Empty;
                    if (r3 != -1) b[r3] = Figure.Empty;

                    // Up direction: positions 19, 11, 3 should be empty
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Empty;
                    if (u2 != -1) b[u2] = Figure.Empty;
                    if (u3 != -1) b[u3] = Figure.Empty;

                    // Down direction: positions 35, 43, 51 should be empty
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D1);
                    var d3 = d2 == -1 ? -1 : d2.GetWithOffset(PositionConstants.D1);
                    if (d1 != -1) b[d1] = Figure.Empty;
                    if (d2 != -1) b[d2] = Figure.Empty;
                    if (d3 != -1) b[d3] = Figure.Empty;
                },
                27, // d4
                Figure.Miner | Figure.IsWhite,
                Miner.GetPossibleActions),

            // Fire walkable scenario
            TestUtils.RunGetActionsScenario(
                "Fire walkable", b =>
                {
                    const int src = 27; // d4
                    // Place Fire squares in all directions - these should be walkable for miner
                    var l1 = src.GetWithOffset(PositionConstants.L1);
                    var r1 = src.GetWithOffset(PositionConstants.R1);
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var d1 = src.GetWithOffset(PositionConstants.D1);

                    if (l1 != -1) b[l1] = Figure.Fire;
                    if (r1 != -1) b[r1] = Figure.Fire;
                    if (u1 != -1) b[u1] = Figure.Fire;
                    if (d1 != -1) b[d1] = Figure.Fire;

                    // Place Fire squares further along to test MinerMove through Fire
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L1);
                    var r2 = r1 == -1 ? -1 : r1.GetWithOffset(PositionConstants.R1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D1);

                    if (l2 != -1) b[l2] = Figure.Fire;
                    if (r2 != -1) b[r2] = Figure.Fire;
                    if (u2 != -1) b[u2] = Figure.Fire;
                    if (d2 != -1) b[d2] = Figure.Fire;
                },
                27, // d4
                Figure.Miner | Figure.IsWhite,
                Miner.GetPossibleActions),

            // Edge case H1 scenario
            TestUtils.RunGetActionsScenario(
                "Edge case H1",
                _ => { /* empty board - only left and up directions available */ },
                7, // h1 (bottom-right corner)
                Figure.Miner | Figure.IsWhite,
                Miner.GetPossibleActions),

            // Edge case A8 scenario
            TestUtils.RunGetActionsScenario(
                "Edge case A8",
                _ => { /* empty board - only right and down directions available */ },
                56, // a8 (top-left corner)
                Figure.Miner | Figure.IsWhite,
                Miner.GetPossibleActions),

            // Edge case H8 scenario
            TestUtils.RunGetActionsScenario(
                "Edge case H8",
                _ => { /* empty board - no movement possible */ },
                63, // h8 (top-right corner)
                Figure.Miner | Figure.IsWhite,
                Miner.GetPossibleActions)
        });
    }

    [Fact]
    public Task ExecuteMove_Verify()
    {
        return Verify(new List<object>
        {
            // Short move no trench scenario
            TestUtils.RunExecuteActionScenario(
                "Short move no trench",
                _ => { /* empty board */ },
                27, // d4
                Figure.Miner | Figure.IsWhite,
                PositionConstants.U1, // Move up one square
                FigureActionType.MinerMove),

            // Long move multiple trenches scenario
            TestUtils.RunExecuteActionScenario(
                "Long move multiple trenches", b =>
                {
                    const int src = 27; // d4
                    // Ensure path is clear for 3 squares up: positions 27->19->11->3
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U1);

                    if (u1 != -1) b[u1] = Figure.Empty;
                    if (u2 != -1) b[u2] = Figure.Empty;
                    if (u3 != -1) b[u3] = Figure.Empty;
                },
                27, // d4
                Figure.Miner | Figure.IsWhite,
                PositionConstants.U3, // Move up three squares
                FigureActionType.MinerMove),

            // With obstacle stops trench scenario
            TestUtils.RunExecuteActionScenario(
                "With obstacle stops trench", b =>
                {
                    const int src = 27; // d4
                    // Place obstacle after 2 squares up, path: 27->19->11 (obstacle)->3 (should not create trench)
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U1);

                    if (u1 != -1) b[u1] = Figure.Empty;
                    if (u2 != -1) b[u2] = Figure.Empty;
                    if (u3 != -1) b[u3] = Figure.Wall; // obstacle stops trench creation
                },
                27, // d4
                Figure.Miner | Figure.IsWhite,
                PositionConstants.U2, // Move up two squares
                FigureActionType.MinerMove),

            // Boundary no trench off board scenario
            TestUtils.RunExecuteActionScenario(
                "Boundary no trench off board",
                _ => { /* empty board */ },
                4, // b8 (top edge, but can move right)
                Figure.Miner | Figure.IsWhite,
                PositionConstants.R1, // Move right one square (to edge)
                FigureActionType.MinerMove),

            // Direction right scenario
            TestUtils.RunExecuteActionScenario(
                "Direction right",
                _ => { /* empty board */ },
                27, // d4
                Figure.Miner | Figure.IsWhite,
                PositionConstants.R1, // Move right one square
                FigureActionType.MinerMove),

            // Direction left scenario
            TestUtils.RunExecuteActionScenario(
                "Direction left",
                _ => { /* empty board */ },
                27, // d4
                Figure.Miner | Figure.IsWhite,
                PositionConstants.L1, // Move left one square
                FigureActionType.MinerMove),

            // Direction up scenario
            TestUtils.RunExecuteActionScenario(
                "Direction up",
                _ => { /* empty board */ },
                27, // d4
                Figure.Miner | Figure.IsWhite,
                PositionConstants.U1, // Move up one square
                FigureActionType.MinerMove),

            // Direction down scenario
            TestUtils.RunExecuteActionScenario(
                "Direction down",
                _ => { /* empty board */ },
                27, // d4
                Figure.Miner | Figure.IsWhite,
                PositionConstants.D1, // Move down one square
                FigureActionType.MinerMove),

            // Edge case corner to corner scenario
            TestUtils.RunExecuteActionScenario(
                "Edge case corner to corner",
                _ => {
                    // Ensure path is clear for diagonal movement to h8 (position 63)
                    // This should create trenches along the diagonal path
                },
                0, // a1 (bottom-left corner)
                Figure.Miner | Figure.IsWhite,
                PositionConstants.U7R7, // Move diagonally to h8 (7 right, 7 up)
                FigureActionType.MinerMove),

            // Edge case along edge scenario
            TestUtils.RunExecuteActionScenario(
                "Edge case along edge", b =>
                {
                    // Ensure path is clear along the bottom edge: positions 0->1->2->3->4->5->6->7
                    for (var i = 1; i < 8; i++) b[i] = Figure.Empty;
                },
                0, // a1 (bottom-left corner)
                Figure.Miner | Figure.IsWhite,
                PositionConstants.R7, // Move right along bottom edge to h1
                FigureActionType.MinerMove),

            // Edge case boundary trench creation scenario
            TestUtils.RunExecuteActionScenario(
                "Edge case boundary trench creation", b =>
                {
                    const int src = 1; // b1 (near bottom edge)
                    // Set up clear path upward but close to right edge
                    // Move up 6 squares: positions 1->9->17->25->33->41->49
                    for (var i = 1; i < 7; i++)
                    {
                        var pos = src.GetWithOffset(PositionConstants.U1);
                        if (pos != -1) b[pos] = Figure.Empty;
                    }
                },
                1, // b1 (near bottom edge)
                Figure.Miner | Figure.IsWhite,
                PositionConstants.U6, // Move up 6 squares toward top edge
                FigureActionType.MinerMove)
        });
    }
}