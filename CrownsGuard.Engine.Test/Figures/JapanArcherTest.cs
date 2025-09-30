using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class JapanArcherTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure japanArcher = Figure.JapanArcher | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty around */ },
                src,
                japanArcher,
                JapanArcher.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Enemy nearby suppresses ranged",
                b =>
                {
                    var upLeft = src.GetWithOffset(PositionConstants.U1L1);
                    if (upLeft != -1) b[upLeft] = Figure.Peasant | Figure.IsBlack; // enemy next to archer
                },
                src,
                japanArcher,
                JapanArcher.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Blocking and enemy within 3",
                b =>
                {
                    // Diagonal (bishop) lines setup
                    // Up-Left: empty then friendly at distance 2
                    var ul1 = src.GetWithOffset(PositionConstants.U1L1);
                    var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.U1L1);
                    if (ul1 != -1) b[ul1] = Figure.Empty; // walkable move
                    if (ul2 != -1) b[ul2] = Figure.Peasant | Figure.IsWhite; // friendly blocks further

                    // Up-Right: enemy at distance 3 with empties before
                    var ur1 = src.GetWithOffset(PositionConstants.U1R1);
                    var ur2 = ur1 == -1 ? -1 : ur1.GetWithOffset(PositionConstants.U1R1);
                    var ur3 = ur2 == -1 ? -1 : ur2.GetWithOffset(PositionConstants.U1R1);
                    if (ur1 != -1) b[ur1] = Figure.Empty; // walkable
                    if (ur2 != -1) b[ur2] = Figure.Empty; // walkable
                    if (ur3 != -1) b[ur3] = Figure.Peasant | Figure.IsBlack; // enemy to shoot

                    // Down-Left: wall immediately
                    var dl1 = src.GetWithOffset(PositionConstants.D1L1);
                    if (dl1 != -1) b[dl1] = Figure.Wall; // non-walkable blocker

                    // Down-Right: friendly immediately
                    var dr1 = src.GetWithOffset(PositionConstants.D1R1);
                    if (dr1 != -1) b[dr1] = Figure.LegionarySword | Figure.IsWhite; // non-walkable blocker
                },
                src,
                japanArcher,
                JapanArcher.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                b => { /* setup places archer at edge via src override */ },
                0, // a1
                japanArcher,
                JapanArcher.GetPossibleActions
            )
        });
    }
}
