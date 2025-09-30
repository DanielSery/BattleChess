using System.Collections.Generic;
using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class LegionarySwordTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty around */ },
                27, // d4
                Figure.LegionarySword | Figure.IsWhite,
                LegionarySword.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Start row double step white",
                b => { /* ensure path ahead is empty by default */ },
                (6 * 8) + 3, // d7
                Figure.LegionarySword | Figure.IsWhite,
                LegionarySword.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Enemy left ally right white",
                b =>
                {
                    var src = 27; // d4
                    var ul = src.GetWithOffset(PositionConstants.UL);
                    var ur = src.GetWithOffset(PositionConstants.UR);
                    if (ul != -1) b[ul] = Figure.Peasant | Figure.IsBlack; // enemy -> MeleeAttack
                    if (ur != -1) b[ur] = Figure.Peasant | Figure.IsWhite; // ally -> MeleeDefend
                },
                27, // d4
                Figure.LegionarySword | Figure.IsWhite,
                LegionarySword.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Forward blocked no move white",
                b =>
                {
                    var src = 27; // d4
                    var forward = src.GetWithOffset(PositionConstants.D); // white forward is D (-Y)
                    if (forward != -1) b[forward] = Figure.Wall; // non-walkable blocks move
                },
                27, // d4
                Figure.LegionarySword | Figure.IsWhite,
                LegionarySword.GetPossibleActions),

            new
            {
                White = TestUtils.RunGetActionsScenario(
                    "Promotion white forward square empty diagonals empty",
                    b => { /* forward square empty, diagonals empty */ },
                    1 * 8 + 3, // d2 -> d1 (row 0)
                    Figure.LegionarySword | Figure.IsWhite,
                    LegionarySword.GetPossibleActions),
                Black = TestUtils.RunGetActionsScenario(
                    "Promotion black forward square empty diagonals empty",
                    b => { /* forward square empty, diagonals empty */ },
                    6 * 8 + 4, // e7 -> e8 (row 7)
                    Figure.LegionarySword | Figure.IsBlack,
                    LegionarySword.GetPossibleActions)
            }
        });
    }
}
            