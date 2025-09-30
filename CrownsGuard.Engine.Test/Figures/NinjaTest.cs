using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class NinjaTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure whiteNinja = Figure.Ninja | Figure.IsWhite;
        const Figure blackNinja = Figure.Ninja | Figure.IsBlack;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "White - All empty", _ => { /* empty around */ },
                src,
                whiteNinja,
                Ninja.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - All empty", _ => { /* empty around */ },
                src,
                blackNinja,
                Ninja.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "White - Attack adjacent", b =>
                {
                    // Place enemies adjacent in all directions
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Peasant | Figure.IsBlack;

                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Archer | Figure.IsBlack;

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Knight | Figure.IsBlack;

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Mage | Figure.IsBlack;
                },
                src,
                whiteNinja,
                Ninja.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - Attack adjacent", b =>
                {
                    // Place enemies adjacent in all directions
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Peasant | Figure.IsWhite;

                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Archer | Figure.IsWhite;

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Knight | Figure.IsWhite;

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Mage | Figure.IsWhite;
                },
                src,
                blackNinja,
                Ninja.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "White - Movement and jump", b =>
                {
                    // Setup for white ninja movement (backward/up)
                    var upLeft = src.GetWithOffset(PositionConstants.D1L1);
                    if (upLeft != -1) b[upLeft] = Figure.Empty; // Regular move

                    var upRight = src.GetWithOffset(PositionConstants.D1R1);
                    if (upRight != -1) b[upRight] = Figure.Empty; // Regular move

                    // Setup jump move: ally then empty space
                    var up1 = src.GetWithOffset(PositionConstants.D1);
                    if (up1 != -1) b[up1] = Figure.Peasant | Figure.IsWhite; // Ally to jump over

                    var up2 = up1 == -1 ? -1 : up1.GetWithOffset(PositionConstants.D1);
                    if (up2 != -1) b[up2] = Figure.Empty; // Empty space to land on
                },
                src,
                whiteNinja,
                Ninja.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - Movement and jump", b =>
                {
                    // Setup for black ninja movement (forward/down)
                    var downLeft = src.GetWithOffset(PositionConstants.U1L1);
                    if (downLeft != -1) b[downLeft] = Figure.Empty; // Regular move

                    var downRight = src.GetWithOffset(PositionConstants.U1R1);
                    if (downRight != -1) b[downRight] = Figure.Empty; // Regular move

                    // Setup jump move: ally then empty space
                    var down1 = src.GetWithOffset(PositionConstants.U1);
                    if (down1 != -1) b[down1] = Figure.Archer | Figure.IsBlack; // Ally to jump over

                    var down2 = down1 == -1 ? -1 : down1.GetWithOffset(PositionConstants.U1);
                    if (down2 != -1) b[down2] = Figure.Empty; // Empty space to land on
                },
                src,
                blackNinja,
                Ninja.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("White - Edge case H1", _ => { /* setup below places ninja at edge via src override */ },
                7, // h1
                whiteNinja,
                Ninja.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Black - Edge case A8", _ => { /* setup below places ninja at edge via src override */ },
                56, // a8
                blackNinja,
                Ninja.GetPossibleActions
            )
        });
    }
}