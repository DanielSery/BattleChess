using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class PeasantTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure whitePeasant = Figure.Peasant | Figure.IsWhite;
        const Figure blackPeasant = Figure.Peasant | Figure.IsBlack;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "White - All empty", _ => { /* empty around */ },
                src,
                whitePeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - All empty", _ => { /* empty around */ },
                src,
                blackPeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "White - Movement and attacks", b =>
                {
                    // White peasant moves up (backward) and attacks up/left/right
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Empty; // Can move here

                    var upLeft = src.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));
                    if (upLeft != -1) b[upLeft] = Figure.Peasant | Figure.IsBlack; // Can attack enemy

                    var upRight = src.GetWithOffset((short)(1 + -1 * PositionConstants.YOffset));
                    if (upRight != -1) b[upRight] = Figure.Archer | Figure.IsBlack; // Can attack enemy

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Knight | Figure.IsBlack; // Can attack enemy

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Mage | Figure.IsBlack; // Can attack enemy

                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Builder | Figure.IsWhite; // Friendly - cannot attack
                },
                src,
                whitePeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - Movement and attacks", b =>
                {
                    // Black peasant moves down (forward) and attacks down/left/right
                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Empty; // Can move here

                    var downLeft = src.GetWithOffset((short)(-1 + 1 * PositionConstants.YOffset));
                    if (downLeft != -1) b[downLeft] = Figure.Peasant | Figure.IsWhite; // Can attack enemy

                    var downRight = src.GetWithOffset((short)(1 + 1 * PositionConstants.YOffset));
                    if (downRight != -1) b[downRight] = Figure.Archer | Figure.IsWhite; // Can attack enemy

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Knight | Figure.IsWhite; // Can attack enemy

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Mage | Figure.IsWhite; // Can attack enemy

                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Builder | Figure.IsBlack; // Friendly - cannot attack
                },
                src,
                blackPeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "White - Blocked movement", b =>
                {
                    // Block white peasant's movement and some attacks
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Wall; // Cannot move through wall

                    var upLeft = src.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));
                    if (upLeft != -1) b[upLeft] = Figure.Archer | Figure.IsWhite; // Friendly - cannot attack

                    var upRight = src.GetWithOffset((short)(1 + -1 * PositionConstants.YOffset));
                    if (upRight != -1) b[upRight] = Figure.Peasant | Figure.IsBlack; // Can attack enemy

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Knight | Figure.IsBlack; // Can attack enemy
                },
                src,
                whitePeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("White - Edge case", _ => { /* setup below places peasant at edge via src override */ },
                7, // h1
                whitePeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Black - Edge case", _ => { /* setup below places peasant at edge via src override */ },
                56, // a8
                blackPeasant,
                Peasant.GetPossibleActions
            )
        });
    }
}