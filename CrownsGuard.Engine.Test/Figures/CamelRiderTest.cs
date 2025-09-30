using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class CamelRiderTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure camelRider = Figure.CamelRider | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty around */ },
                src,
                camelRider,
                CamelRider.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Enemy on diagonal", b =>
                {
                    // Place enemy two steps up-left (diagonal) with empty in between
                    var ul1 = src.GetWithOffset(PositionConstants.U1L1);
                    var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.U1L1);
                    if (ul1 != -1) b[ul1] = Figure.Empty;
                    if (ul2 != -1) b[ul2] = Figure.Peasant | Figure.IsBlack;

                    // Up-right: friendly at distance 3, with two empties before
                    var ur1 = src.GetWithOffset(PositionConstants.U1R1);
                    var ur2 = ur1 == -1 ? -1 : ur1.GetWithOffset(PositionConstants.U1R1);
                    var ur3 = ur2 == -1 ? -1 : ur2.GetWithOffset(PositionConstants.U1R1);
                    if (ur1 != -1) b[ur1] = Figure.Empty;
                    if (ur2 != -1) b[ur2] = Figure.Empty;
                    if (ur3 != -1) b[ur3] = Figure.LegionarySword | Figure.IsWhite; // should cause MeleeDefend

                    // Down-left: wall immediately blocks
                    var dl1 = src.GetWithOffset(PositionConstants.D1L1);
                    if (dl1 != -1) b[dl1] = Figure.Wall;

                    // Down-right: enemy adjacent
                    var dr1 = src.GetWithOffset(PositionConstants.D1R1);
                    if (dr1 != -1) b[dr1] = Figure.Peasant | Figure.IsBlack;
                },
                src,
                camelRider,
                CamelRider.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1", _ => { /* source fixed to edge via src index */ },
                0, // a1
                camelRider,
                CamelRider.GetPossibleActions)
        });
    }
}
