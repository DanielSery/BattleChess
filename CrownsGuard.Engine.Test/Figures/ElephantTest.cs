using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class ElephantTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure elephant = Figure.Elephant | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty board */ },
                src,
                elephant,
                Elephant.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Mixed blocking and attacks", b =>
                {
                    // Up direction: empty, then enemy, then whatever after becomes attack chain
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Empty; // should allow Move and PossibleMeleePierceAttack
                    if (u2 != -1) b[u2] = Figure.Peasant | Figure.IsBlack; // first attack square
                    // u3: even if empty, attack should continue due to isAttack=true
                    if (u3 != -1) b[u3] = Figure.Empty;

                    // Down direction: immediate wall block, then continue attacks for next two tiles
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D1);
                    var d3 = d2 == -1 ? -1 : d2.GetWithOffset(PositionConstants.D1);
                    if (d1 != -1) b[d1] = Figure.Wall; // non-walkable triggers attack at d1
                    if (d2 != -1) b[d2] = Figure.Empty; // still should be counted as attack
                    if (d3 != -1) b[d3] = Figure.Peasant | Figure.IsBlack; // still attack
                },
                src,
                elephant,
                Elephant.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Enemy adjacent above and below", b =>
                {
                    // Enemy immediately above
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Peasant | Figure.IsBlack;
                    if (u2 != -1) b[u2] = Figure.Empty; // still attack mode
                    if (u3 != -1) b[u3] = Figure.Empty; // still attack mode

                    // Enemy immediately below
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D1);
                    var d3 = d2 == -1 ? -1 : d2.GetWithOffset(PositionConstants.D1);
                    if (d1 != -1) b[d1] = Figure.Peasant | Figure.IsBlack;
                    if (d2 != -1) b[d2] = Figure.LegionarySword | Figure.IsBlack; // any piece, still attack mode
                    if (d3 != -1) b[d3] = Figure.Empty; // still attack mode
                },
                src,
                elephant,
                Elephant.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                _ => { /* edge via src index */ },
                0, // a1
                elephant,
                Elephant.GetPossibleActions)
        });
    }
}
