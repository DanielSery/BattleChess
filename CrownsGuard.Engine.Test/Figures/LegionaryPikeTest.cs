using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class LegionaryPikeTest
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
                Figure.LegionaryPike | Figure.IsWhite,
                LegionaryPike.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Mixed blocking and attacks", b =>
                {
                    const int src = 27; // d4
                    // Melee (white forward-diagonals): one enemy and one friendly adjacent
                    var ul = src.GetWithOffset(PositionConstants.UL);
                    var ur = src.GetWithOffset(PositionConstants.UR);
                    if (ul != -1) b[ul] = Figure.Peasant | Figure.IsBlack; // enemy - melee attack possible
                    if (ur != -1) b[ur] = Figure.LegionarySword | Figure.IsWhite; // friendly - only PossibleMeleeAttack

                    // Pike ranged (two forward diagonals): one enemy and one non-attackable (friendly) to produce PossibleRangedAttack
                    var uul = src.GetWithOffset(unchecked((byte)-1) - 2 * PositionConstants.YOffset);
                    var uur = src.GetWithOffset(unchecked((byte)+1) - 2 * PositionConstants.YOffset);
                    if (uul != -1) b[uul] = Figure.Peasant | Figure.IsBlack; // enemy - ranged attack
                    if (uur != -1) b[uur] = Figure.LegionarySword | Figure.IsWhite; // friendly - only PossibleRangedAttack

                    // Forward moves: place a blocker immediately ahead to block both single and double moves
                    var u = src.GetWithOffset(PositionConstants.D); // for white forward is down (negative Y)
                    // Note: in engine, white forward move uses -1 * YOffset (PositionConstants.D)
                    if (u != -1) b[u] = Figure.Wall; // non-walkable blocker
                },
                27, // d4
                Figure.LegionaryPike | Figure.IsWhite,
                LegionaryPike.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Start double step white",
                b => { /* ensure path is empty */ },
                6 * 8 + 3, // d7
                Figure.LegionaryPike | Figure.IsWhite,
                LegionaryPike.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Start double step black",
                b => { /* ensure path is empty */ },
                1 * 8 + 4, // e2
                Figure.LegionaryPike | Figure.IsBlack,
                LegionaryPike.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                b => { /* setup places pike at edge via src override */ },
                0, // a1
                Figure.LegionaryPike | Figure.IsWhite,
                LegionaryPike.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Promotion to queen white",
                b => { /* forward empty */ },
                1 * 8 + 3, // d2 -> d1
                Figure.LegionaryPike | Figure.IsWhite,
                LegionaryPike.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Promotion to queen black",
                b => { /* forward empty */ },
                6 * 8 + 4, // e7 -> e8
                Figure.LegionaryPike | Figure.IsBlack,
                LegionaryPike.GetPossibleActions)
        });
    }
}
