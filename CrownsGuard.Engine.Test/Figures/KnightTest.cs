using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class KnightTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure knight = Figure.Knight | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty around */ }, src, knight, Knight.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Mixed blocking and attacks", b =>
                {
                    // Left: empty then friendly blocker at distance 2
                    var l1 = src.GetWithOffset(PositionConstants.L);
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L);
                    if (l1 != -1) b[l1] = Figure.Empty; // walkable
                    if (l2 != -1) b[l2] = Figure.Peasant | Figure.IsWhite; // friendly blocks

                    // Up: enemy at distance 3
                    var u1 = src.GetWithOffset(PositionConstants.U);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U);
                    if (u1 != -1) b[u1] = Figure.Empty; // walkable
                    if (u2 != -1) b[u2] = Figure.Empty; // walkable
                    if (u3 != -1) b[u3] = Figure.Peasant | Figure.IsBlack; // enemy to strike

                    // Right: wall immediately
                    var r1 = src.GetWithOffset(PositionConstants.R);
                    if (r1 != -1) b[r1] = Figure.Wall; // non-walkable blocker

                    // Down: friendly immediately
                    var d1 = src.GetWithOffset(PositionConstants.D);
                    if (d1 != -1) b[d1] = Figure.LegionarySword | Figure.IsWhite; // non-walkable blocker
                }, src, knight, Knight.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                _ => { },
                0, // a1
                knight,
                Knight.GetPossibleActions)
        });
    }
}
