using System.Collections.Generic;
using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class BladeTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Verify()
    {
        const int src = 27; // d4
        const Figure blade = Figure.Blade | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "AllEmpty",
            _ => { /* empty around */ },
            src,
            blade,
            Blade.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_BlockingAndEnemyWithin3_Verify()
    {
        const int src = 27; // d4
        const Figure blade = Figure.Blade | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "BlockingAndEnemyWithin3",
            b =>
            {
                // Up-Left: enemy at distance 2, then wall at distance 3 (should still add possible at 3 because non-empty)
                var ul1 = src.GetWithOffset(PositionConstants.UL);
                var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.UL);
                var ul3 = ul2 == -1 ? -1 : ul2.GetWithOffset(PositionConstants.UL);
                if (ul2 != -1) b[ul2] = Figure.Peasant | Figure.IsBlack; // enemy to pierce
                if (ul3 != -1) b[ul3] = Figure.Wall; // non-walkable blocker after enemy

                // Right: friendly immediately blocks (should add Possible at 1 and stop)
                var r1 = src.GetWithOffset(PositionConstants.R);
                if (r1 != -1) b[r1] = Figure.LegionarySword | Figure.IsWhite;

                // Down: empty, empty, enemy at 3 (attack at 3)
                var d1 = src.GetWithOffset(PositionConstants.D);
                var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D);
                var d3 = d2 == -1 ? -1 : d2.GetWithOffset(PositionConstants.D);
                if (d1 != -1) b[d1] = Figure.Empty;
                if (d2 != -1) b[d2] = Figure.Empty;
                if (d3 != -1) b[d3] = Figure.Scout | Figure.IsBlack;

                // Left: wall at distance 2 (should add Possible at 1, then Possible at 2 and stop)
                var l1 = src.GetWithOffset(PositionConstants.L);
                var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L);
                if (l2 != -1) b[l2] = Figure.Wall;

                // Up: enemy immediately (attack at 1), then empty at 2 (still collect possible after attack because Blade continues checking)
                var u1 = src.GetWithOffset(PositionConstants.U);
                var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                if (u1 != -1) b[u1] = Figure.Bard | Figure.IsBlack;
                if (u2 != -1) b[u2] = Figure.Empty;
            },
            src,
            blade,
            Blade.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_AdjacentEnemiesAffectMoves_Verify()
    {
        const int src = 27; // d4
        const Figure blade = Figure.Blade | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "AdjacentEnemiesAffectMoves",
            b =>
            {
                // Place enemies in several adjacent tiles; moves should only include walkable (empty) neighbors
                var dirs = PositionConstants.QueenDirections;
                foreach (var rel in dirs)
                {
                    var idx = src.GetWithOffset(rel);
                    if (idx != -1) b[idx] = Figure.Peasant | Figure.IsBlack;
                }
                // Make two tiles walkable to allow some moves
                var upIdx = src.GetWithOffset(PositionConstants.U);
                var rightIdx = src.GetWithOffset(PositionConstants.R);
                if (upIdx != -1) b[upIdx] = Figure.Empty;
                if (rightIdx != -1) b[rightIdx] = Figure.Empty;
            },
            src,
            blade,
            Blade.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_Verify()
    {
        const Figure blade = Figure.Blade | Figure.IsWhite;
        return Verify(TestUtils.RunGetActionsScenario(
            "EdgeCase_A1",
            b => { /* archer placed at corner via src */ },
            0, // a1
            blade,
            Blade.GetPossibleActions
        ));
    }
}
