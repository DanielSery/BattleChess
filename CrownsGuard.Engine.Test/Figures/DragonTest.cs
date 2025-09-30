using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class DragonTest
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
                Figure.Dragon | Figure.IsWhite,
                Dragon.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Enemy adjacent suppresses breath",
                b =>
                {
                    // Any adjacent enemy in queen directions suppresses breathe fire generation
                    var ur = 27.GetWithOffset(PositionConstants.U1R1);
                    if (ur != -1) b[ur] = Figure.Peasant | Figure.IsBlack;
                },
                27, // d4
                Figure.Dragon | Figure.IsWhite,
                Dragon.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Mixed scenario",
                b =>
                {
                    // UL: empty at 1, wall at 2 -> should add BreatheFire at 1 only
                    var ul1 = 27.GetWithOffset(PositionConstants.U1L1);
                    var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.U1L1);
                    if (ul2 != -1) b[ul2] = Figure.Wall;

                    // UR: friendly at 1 -> no BreatheFire on that ray
                    var ur1 = 27.GetWithOffset(PositionConstants.U1R1);
                    if (ur1 != -1) b[ur1] = Figure.LegionarySword | Figure.IsWhite;

                    // DL: empty at 1 and 2 -> should add BreatheFire at both
                    // (leave empty)

                    // DR: enemy at 2 (with empty at 1) -> no BreatheFire at 2, but add at 1
                    var dr1 = 27.GetWithOffset(PositionConstants.D1R1);
                    var dr2 = dr1 == -1 ? -1 : dr1.GetWithOffset(PositionConstants.D1R1);
                    if (dr2 != -1) b[dr2] = Figure.Knight | Figure.IsBlack;

                    // Rook neighbors: ensure some are blocked and some are empty
                    var l = 27.GetWithOffset(PositionConstants.L1);
                    if (l != -1) b[l] = Figure.Empty; // move allowed
                    var r = 27.GetWithOffset(PositionConstants.R1);
                    if (r != -1) b[r] = Figure.Wall; // no move right
                },
                27, // d4
                Figure.Dragon | Figure.IsWhite,
                Dragon.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                _ => { },
                0, // a1
                Figure.Dragon | Figure.IsWhite,
                Dragon.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case H8 Black",
                _ => { },
                63, // h8
                Figure.Dragon | Figure.IsBlack,
                Dragon.GetPossibleActions)
        });
    }

    [Fact]
    public Task ExecuteBreatheFire_Verify()
    {
        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario(
                "Adjacent target",
                b =>
                {
                    // destination must be empty for BreatheFire action creation; executor will create Fire there
                    var dst = 27.GetWithOffset(PositionConstants.U1L1);
                    if (dst != -1) b[dst] = Figure.Empty;
                },
                27, // d4
                Figure.Dragon | Figure.IsWhite,
                PositionConstants.U1L1,
                FigureActionType.BreatheFire),

            TestUtils.RunExecuteActionScenario(
                "Two-step target",
                b =>
                {
                    var mid = 27.GetWithOffset(PositionConstants.U1L1);
                    var dst = mid == -1 ? -1 : mid.GetWithOffset(PositionConstants.U1L1);
                    if (mid != -1) b[mid] = Figure.Empty; // mid must be empty per generator
                    if (dst != -1) b[dst] = Figure.Empty;
                },
                27, // d4
                Figure.Dragon | Figure.IsWhite,
                PositionConstants.U2L2,
                FigureActionType.BreatheFire)
        });
    }
}
