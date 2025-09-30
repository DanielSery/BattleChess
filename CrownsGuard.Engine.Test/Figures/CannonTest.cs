using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class CannonTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* no blockers, no adjacent enemies */ },
                27, // d4
                Figure.Cannon | Figure.IsWhite,
                Cannon.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Enemy adjacent suppresses all", b =>
                {
                    var up = 27.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Peasant | Figure.IsBlack; // any adjacent enemy suppresses all actions
                },
                27, // d4
                Figure.Cannon | Figure.IsWhite,
                Cannon.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Mix possible and actual", b =>
                {
                    var src = 27; // d4
                    // Place pieces exactly at 2-4 squares forward (white moves up)
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U1);
                    var u4 = u3 == -1 ? -1 : u3.GetWithOffset(PositionConstants.U1);
                    if (u2 != -1) b[u2] = Figure.Peasant | Figure.IsBlack; // enemy -> CannonAttack
                    if (u3 != -1) b[u3] = Figure.Peasant | Figure.IsWhite; // friendly -> PossibleCannonAttack
                    if (u4 != -1) b[u4] = Figure.Wall; // wall -> PossibleCannonAttack
                },
                27, // d4
                Figure.Cannon | Figure.IsWhite,
                Cannon.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Edge case A1 white", _ => { },
                0, // a1
                Figure.Cannon | Figure.IsWhite,
                Cannon.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Edge case H8 black", _ => { },
                63, // h8
                Figure.Cannon | Figure.IsBlack,
                Cannon.GetPossibleActions
            )
        });
    }

    [Fact]
    public Task ExecuteAttack_Verify()
    {
        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario(
                "White kills at 2-4 forward", b =>
                {
                    var src = 27; // d4
                    // Enemies at 2 and 3 up; empty at 4 up
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U1);
                    var u4 = u3 == -1 ? -1 : u3.GetWithOffset(PositionConstants.U1);
                    if (u2 != -1) b[u2] = Figure.Knight | Figure.IsBlack;
                    if (u3 != -1) b[u3] = Figure.Archer | Figure.IsBlack;
                    // leave u4 empty
                },
                27, // d4
                Figure.Cannon | Figure.IsWhite,
                // Use relative of two steps up to target a valid CannonAttack square
                (short)(unchecked((byte)+0) - 2*PositionConstants.YOffset),
                FigureActionType.CannonAttack
            ),

            TestUtils.RunExecuteActionScenario(
                "Black kills at 2-4 forward", b =>
                {
                    var src = 36; // e5 (somewhere central)
                    // For black, forward is down (+YOffset)
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D1);
                    var d3 = d2 == -1 ? -1 : d2.GetWithOffset(PositionConstants.D1);
                    var d4 = d3 == -1 ? -1 : d3.GetWithOffset(PositionConstants.D1);
                    if (d2 != -1) b[d2] = Figure.Trader | Figure.IsWhite;
                    if (d4 != -1) b[d4] = Figure.Wall; // non-empty also gets destroyed
                },
                36, // e5
                Figure.Cannon | Figure.IsBlack,
                (short)(unchecked((byte)+0) + 2*PositionConstants.YOffset),
                FigureActionType.CannonAttack
            )
        });
    }
}
