using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class DragonTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_White_Verify()
    {
        const int src = 27; // d4
        const Figure dragon = Figure.Dragon | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "AllEmpty_White",
            _ => { /* empty around */ },
            src,
            dragon,
            Dragon.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EnemyAdjacentSuppressBreath_Verify()
    {
        const int src = 27; // d4
        const Figure dragon = Figure.Dragon | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "EnemyAdjacentSuppressBreath",
            b =>
            {
                // Any adjacent enemy in queen directions suppresses breathe fire generation
                var ur = src.GetWithOffset(PositionConstants.UR);
                if (ur != -1) b[ur] = Figure.Peasant | Figure.IsBlack;
            },
            src,
            dragon,
            Dragon.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_Mixed_White_Verify()
    {
        const int src = 27; // d4
        const Figure dragon = Figure.Dragon | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "Mixed_White",
            b =>
            {
                // UL: empty at 1, wall at 2 -> should add BreatheFire at 1 only
                var ul1 = src.GetWithOffset(PositionConstants.UL);
                var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.UL);
                if (ul2 != -1) b[ul2] = Figure.Wall;

                // UR: friendly at 1 -> no BreatheFire on that ray
                var ur1 = src.GetWithOffset(PositionConstants.UR);
                if (ur1 != -1) b[ur1] = Figure.LegionarySword | Figure.IsWhite;

                // DL: empty at 1 and 2 -> should add BreatheFire at both
                // (leave empty)

                // DR: enemy at 2 (with empty at 1) -> no BreatheFire at 2, but add at 1
                var dr1 = src.GetWithOffset(PositionConstants.DR);
                var dr2 = dr1 == -1 ? -1 : dr1.GetWithOffset(PositionConstants.DR);
                if (dr2 != -1) b[dr2] = Figure.Knight | Figure.IsBlack;

                // Rook neighbors: ensure some are blocked and some are empty
                var l = src.GetWithOffset(PositionConstants.L);
                if (l != -1) b[l] = Figure.Empty; // move allowed
                var r = src.GetWithOffset(PositionConstants.R);
                if (r != -1) b[r] = Figure.Wall; // no move right
            },
            src,
            dragon,
            Dragon.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_White_Verify()
    {
        const int src = 0; // a1
        const Figure dragon = Figure.Dragon | Figure.IsWhite;
        return Verify(TestUtils.RunGetActionsScenario(
            "EdgeCase_A1_White",
            _ => { },
            src,
            dragon,
            Dragon.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_H8_Black_Verify()
    {
        const int src = 63; // h8
        const Figure dragon = Figure.Dragon | Figure.IsBlack;
        return Verify(TestUtils.RunGetActionsScenario(
            "EdgeCase_H8_Black",
            _ => { },
            src,
            dragon,
            Dragon.GetPossibleActions
        ));
    }

    [Fact]
    public Task ExecuteBreatheFire_Adjacent_White_Verify()
    {
        const int src = 27; // d4
        const Figure dragon = Figure.Dragon | Figure.IsWhite;

        // choose an adjacent diagonal target: UL relative (one step)
        short relative = PositionConstants.UL;

        return Verify(TestUtils.RunExecuteActionScenario(
            "ExecuteBreatheFire_Adjacent_White",
            b =>
            {
                // destination must be empty for BreatheFire action creation; executor will create Fire there
                var dst = src.GetWithOffset(relative);
                if (dst != -1) b[dst] = Figure.Empty;
            },
            src,
            dragon,
            relative,
            FigureActionType.BreatheFire
        ));
    }

    [Fact]
    public Task ExecuteBreatheFire_TwoSteps_White_Verify()
    {
        const int src = 27; // d4
        const Figure dragon = Figure.Dragon | Figure.IsWhite;

        // two-step diagonal: UL + UL
        short relative = (short)(PositionConstants.UL + PositionConstants.UL);

        return Verify(TestUtils.RunExecuteActionScenario(
            "ExecuteBreatheFire_TwoSteps_White",
            b =>
            {
                var mid = src.GetWithOffset(PositionConstants.UL);
                var dst = mid == -1 ? -1 : mid.GetWithOffset(PositionConstants.UL);
                if (mid != -1) b[mid] = Figure.Empty; // mid must be empty per generator
                if (dst != -1) b[dst] = Figure.Empty;
            },
            src,
            dragon,
            relative,
            FigureActionType.BreatheFire
        ));
    }
}
