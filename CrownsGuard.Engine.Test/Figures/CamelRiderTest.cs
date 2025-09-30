using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
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
                    var ul1 = src.GetWithOffset(PositionConstants.UL);
                    var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.UL);
                    if (ul1 != -1) b[ul1] = Figure.Empty;
                    if (ul2 != -1) b[ul2] = Figure.Peasant | Figure.IsBlack;

                    // Up-right: friendly at distance 3, with two empties before
                    var ur1 = src.GetWithOffset(PositionConstants.UR);
                    var ur2 = ur1 == -1 ? -1 : ur1.GetWithOffset(PositionConstants.UR);
                    var ur3 = ur2 == -1 ? -1 : ur2.GetWithOffset(PositionConstants.UR);
                    if (ur1 != -1) b[ur1] = Figure.Empty;
                    if (ur2 != -1) b[ur2] = Figure.Empty;
                    if (ur3 != -1) b[ur3] = Figure.LegionarySword | Figure.IsWhite; // should cause MeleeDefend

                    // Down-left: wall immediately blocks
                    var dl1 = src.GetWithOffset(PositionConstants.DL);
                    if (dl1 != -1) b[dl1] = Figure.Wall;

                    // Down-right: enemy adjacent
                    var dr1 = src.GetWithOffset(PositionConstants.DR);
                    if (dr1 != -1) b[dr1] = Figure.Peasant | Figure.IsBlack;
                },
                src,
                camelRider,
                CamelRider.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1", b => { /* source fixed to edge via src index */ },
                0, // a1
                camelRider,
                CamelRider.GetPossibleActions)
        });
    }
}
