using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class ArcherTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure archer = Figure.Archer | Figure.IsWhite;

        return Verify(new List< object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                archer,
                Archer.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario(
                "Enemy near", b =>
                {
                    var upLeft = src.GetWithOffset(PositionConstants.U1L1);
                    if (upLeft != -1) b[upLeft] = Figure.Peasant | Figure.IsBlack; // enemy next to archer
                },
                src,
                archer,
                Archer.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario("Blocking within 3", b =>
                {
                    // Left: empty then friendly blocker at distance 2
                    var l1 = src.GetWithOffset(PositionConstants.L1);
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L1);
                    if (l1 != -1) b[l1] = Figure.Empty; // walkable
                    if (l2 != -1) b[l2] = Figure.Peasant | Figure.IsWhite; // friendly blocks

                    // Up: enemy at distance 3
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Empty; // walkable
                    if (u2 != -1) b[u2] = Figure.Empty; // walkable
                    if (u3 != -1) b[u3] = Figure.Peasant | Figure.IsBlack; // enemy to shoot

                    // Right: wall immediately
                    var r1 = src.GetWithOffset(PositionConstants.R1);
                    if (r1 != -1) b[r1] = Figure.Wall; // non-walkable blocker

                    // Down: friendly immediately
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    if (d1 != -1) b[d1] = Figure.LegionarySword | Figure.IsWhite; // non-walkable blocker
                },
                src,
                archer,
                Archer.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario("Edge case", _ => { /* setup below places archer at edge via src override */ },
                0, // a1
                archer,
                Archer.GetPossibleActions
            )
        });
    }
}
