using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class ElephantTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Verify()
    {
        const int src = 27; // d4
        const Figure elephant = Figure.Elephant | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ => { /* empty board */ },
            src,
            elephant,
            Elephant.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_MixedBlockingAndAttacks_Verify()
    {
        const int src = 27; // d4
        const Figure elephant = Figure.Elephant | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // Up direction: empty, then enemy, then whatever after becomes attack chain
                var u1 = src.GetWithOffset(PositionConstants.U);
                var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U);
                if (u1 != -1) b[u1] = Figure.Empty; // should allow Move and PossibleMeleePierceAttack
                if (u2 != -1) b[u2] = Figure.Peasant | Figure.IsBlack; // first attack square
                // u3: even if empty, attack should continue due to isAttack=true
                if (u3 != -1) b[u3] = Figure.Empty;

                // Down direction: immediate wall block, then continue attacks for next two tiles
                var d1 = src.GetWithOffset(PositionConstants.D);
                var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D);
                var d3 = d2 == -1 ? -1 : d2.GetWithOffset(PositionConstants.D);
                if (d1 != -1) b[d1] = Figure.Wall; // non-walkable triggers attack at d1
                if (d2 != -1) b[d2] = Figure.Empty; // still should be counted as attack
                if (d3 != -1) b[d3] = Figure.Peasant | Figure.IsBlack; // still attack
            },
            src,
            elephant,
            Elephant.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EnemyAdjacentAboveAndBelow_Verify()
    {
        const int src = 27; // d4
        const Figure elephant = Figure.Elephant | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // Enemy immediately above
                var u1 = src.GetWithOffset(PositionConstants.U);
                var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U);
                if (u1 != -1) b[u1] = Figure.Peasant | Figure.IsBlack;
                if (u2 != -1) b[u2] = Figure.Empty; // still attack mode
                if (u3 != -1) b[u3] = Figure.Empty; // still attack mode

                // Enemy immediately below
                var d1 = src.GetWithOffset(PositionConstants.D);
                var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D);
                var d3 = d2 == -1 ? -1 : d2.GetWithOffset(PositionConstants.D);
                if (d1 != -1) b[d1] = Figure.Peasant | Figure.IsBlack;
                if (d2 != -1) b[d2] = Figure.LegionarySword | Figure.IsBlack; // any piece, still attack mode
                if (d3 != -1) b[d3] = Figure.Empty; // still attack mode
            },
            src,
            elephant,
            Elephant.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_Verify()
    {
        const Figure elephant = Figure.Elephant | Figure.IsWhite;
        return Verify(TestUtils.RunGetActionsScenario(_ => { /* edge via src index */ },
            0, // a1
            elephant,
            Elephant.GetPossibleActions
        ));
    }
}
