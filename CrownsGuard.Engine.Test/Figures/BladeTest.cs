using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class BladeTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure blade = Figure.Blade | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty around */ },
                src,
                blade,
                Blade.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Blocking and enemy within 3", b =>
                {
                    // Up-Left: enemy at distance 2, then wall at distance 3 (should still add possible at 3 because non-empty)
                    var ul1 = src.GetWithOffset(PositionConstants.U1L1);
                    var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.U1L1);
                    var ul3 = ul2 == -1 ? -1 : ul2.GetWithOffset(PositionConstants.U1L1);
                    if (ul2 != -1) b[ul2] = Figure.Peasant | Figure.IsBlack; // enemy to pierce
                    if (ul3 != -1) b[ul3] = Figure.Wall; // non-walkable blocker after enemy

                    // Right: friendly immediately blocks (should add Possible at 1 and stop)
                    var r1 = src.GetWithOffset(PositionConstants.R1);
                    if (r1 != -1) b[r1] = Figure.LegionarySword | Figure.IsWhite;

                    // Down: empty, empty, enemy at 3 (attack at 3)
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D1);
                    var d3 = d2 == -1 ? -1 : d2.GetWithOffset(PositionConstants.D1);
                    if (d1 != -1) b[d1] = Figure.Empty;
                    if (d2 != -1) b[d2] = Figure.Empty;
                    if (d3 != -1) b[d3] = Figure.Scout | Figure.IsBlack;

                    // Left: wall at distance 2 (should add Possible at 1, then Possible at 2 and stop)
                    var l1 = src.GetWithOffset(PositionConstants.L1);
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L1);
                    if (l2 != -1) b[l2] = Figure.Wall;

                    // Up: enemy immediately (attack at 1), then empty at 2 (still collect possible after attack because Blade continues checking)
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Bard | Figure.IsBlack;
                    if (u2 != -1) b[u2] = Figure.Empty;
                },
                src,
                blade,
                Blade.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Adjacent enemies affect moves", b =>
                {
                    // Place enemies in several adjacent tiles; moves should only include walkable (empty) neighbors
                    var dirs = PositionConstants.QueenDirections;
                    foreach (var rel in dirs)
                    {
                        var idx = src.GetWithOffset(rel);
                        if (idx != -1) b[idx] = Figure.Peasant | Figure.IsBlack;
                    }
                    // Make two tiles walkable to allow some moves
                    var upIdx = src.GetWithOffset(PositionConstants.U1);
                    var rightIdx = src.GetWithOffset(PositionConstants.R1);
                    if (upIdx != -1) b[upIdx] = Figure.Empty;
                    if (rightIdx != -1) b[rightIdx] = Figure.Empty;
                },
                src,
                blade,
                Blade.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                _ => { /* blade placed at corner via src */ },
                0, // a1
                blade,
                Blade.GetPossibleActions
            )
        });
    }
}
