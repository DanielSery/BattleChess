using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class SpartanTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure spartan = Figure.Spartan | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                spartan,
                Spartan.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Movement in all directions", b =>
                {
                    // Clear all adjacent squares for movement
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Empty;

                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Empty;

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Empty;

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Empty;

                    var ul = src.GetWithOffset(PositionConstants.U1L1);
                    if (ul != -1) b[ul] = Figure.Empty;

                    var ur = src.GetWithOffset(PositionConstants.U1R1);
                    if (ur != -1) b[ur] = Figure.Empty;

                    var dl = src.GetWithOffset(PositionConstants.D1L1);
                    if (dl != -1) b[dl] = Figure.Empty;

                    var dr = src.GetWithOffset(PositionConstants.D1R1);
                    if (dr != -1) b[dr] = Figure.Empty;
                },
                src,
                spartan,
                Spartan.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case A1", _ => { /* setup below places spartan at edge via src override */ },
                0, // a1
                spartan,
                Spartan.GetPossibleActions
            )
        });
    }

    [Fact]
    public Task ExecuteMove_ChargeAttack_Verify()
    {
        const int src = 27; // d4
        const Figure spartan = Figure.Spartan | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario(
                "Horizontal charge right", b =>
                {
                    // Setup: spartan moves right, enemy immediately behind target
                    var targetIndex = src.GetWithOffset(PositionConstants.R1);
                    var chargeTarget = targetIndex == -1 ? -1 : targetIndex.GetWithOffset(PositionConstants.R1);

                    if (targetIndex != -1) b[targetIndex] = Figure.Empty; // Target square
                    if (chargeTarget != -1) b[chargeTarget] = Figure.Peasant | Figure.IsBlack; // Enemy to charge into
                },
                src,
                spartan,
                PositionConstants.R1,
                FigureActionType.SpartanMove
            ),

            TestUtils.RunExecuteActionScenario(
                "Vertical charge up", b =>
                {
                    // Setup: spartan moves up, enemy immediately behind target
                    var targetIndex = src.GetWithOffset(PositionConstants.U1);
                    var chargeTarget = targetIndex == -1 ? -1 : targetIndex.GetWithOffset(PositionConstants.U1);

                    if (targetIndex != -1) b[targetIndex] = Figure.Empty; // Target square
                    if (chargeTarget != -1) b[chargeTarget] = Figure.Archer | Figure.IsBlack; // Enemy to charge into
                },
                src,
                spartan,
                PositionConstants.U1,
                FigureActionType.SpartanMove
            ),

            TestUtils.RunExecuteActionScenario(
                "Diagonal charge", b =>
                {
                    // Setup: spartan moves diagonally, enemy immediately behind target
                    var targetIndex = src.GetWithOffset(PositionConstants.U1R1);
                    var chargeTarget = targetIndex == -1 ? -1 : targetIndex.GetWithOffset(PositionConstants.U1R1);

                    if (targetIndex != -1) b[targetIndex] = Figure.Empty; // Target square
                    if (chargeTarget != -1) b[chargeTarget] = Figure.Knight | Figure.IsBlack; // Enemy to charge into
                },
                src,
                spartan,
                PositionConstants.U1R1,
                FigureActionType.SpartanMove
            ),

            TestUtils.RunExecuteActionScenario(
                "No charge - no enemy behind", b =>
                {
                    // Setup: spartan moves, but no enemy behind target
                    var targetIndex = src.GetWithOffset(PositionConstants.R1);
                    if (targetIndex != -1) b[targetIndex] = Figure.Empty; // Target square only
                },
                src,
                spartan,
                PositionConstants.R1,
                FigureActionType.SpartanMove),
        });
    }
}