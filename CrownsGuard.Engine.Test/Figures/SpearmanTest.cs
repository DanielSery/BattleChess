using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class SpearmanTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure whiteSpearman = Figure.Spearman | Figure.IsWhite;
        const Figure blackSpearman = Figure.Spearman | Figure.IsBlack;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "White - All empty", _ => { /* empty around */ },
                src,
                whiteSpearman,
                Spearman.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - All empty", _ => { /* empty around */ },
                src,
                blackSpearman,
                Spearman.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "White - Movement and attacks", b =>
                {
                    // White spearman moves backward (up) and attacks diagonally
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Empty; // Can move here

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Peasant | Figure.IsBlack; // Can attack enemy

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Archer | Figure.IsBlack; // Can attack enemy

                    // Diagonal attacks
                    var upLeft = src.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));
                    if (upLeft != -1) b[upLeft] = Figure.Knight | Figure.IsBlack; // Can attack enemy

                    var upRight = src.GetWithOffset((short)(1 + -1 * PositionConstants.YOffset));
                    if (upRight != -1) b[upRight] = Figure.Mage | Figure.IsBlack; // Can attack enemy

                    var downLeft = src.GetWithOffset((short)(-1 + 1 * PositionConstants.YOffset));
                    if (downLeft != -1) b[downLeft] = Figure.Builder | Figure.IsBlack; // Can attack enemy

                    var downRight = src.GetWithOffset((short)(1 + 1 * PositionConstants.YOffset));
                    if (downRight != -1) b[downRight] = Figure.Dragon | Figure.IsBlack; // Can attack enemy

                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Barbarian | Figure.IsWhite; // Friendly - cannot attack
                },
                src,
                whiteSpearman,
                Spearman.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - Movement and attacks", b =>
                {
                    // Black spearman moves forward (down) and attacks diagonally
                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Empty; // Can move here

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Peasant | Figure.IsWhite; // Can attack enemy

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Archer | Figure.IsWhite; // Can attack enemy

                    // Diagonal attacks
                    var upLeft = src.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));
                    if (upLeft != -1) b[upLeft] = Figure.Knight | Figure.IsWhite; // Can attack enemy

                    var upRight = src.GetWithOffset((short)(1 + -1 * PositionConstants.YOffset));
                    if (upRight != -1) b[upRight] = Figure.Mage | Figure.IsWhite; // Can attack enemy

                    var downLeft = src.GetWithOffset((short)(-1 + 1 * PositionConstants.YOffset));
                    if (downLeft != -1) b[downLeft] = Figure.Builder | Figure.IsWhite; // Can attack enemy

                    var downRight = src.GetWithOffset((short)(1 + 1 * PositionConstants.YOffset));
                    if (downRight != -1) b[downRight] = Figure.Dragon | Figure.IsWhite; // Can attack enemy

                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Barbarian | Figure.IsBlack; // Friendly - cannot attack
                },
                src,
                blackSpearman,
                Spearman.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "White - Blocked movement", b =>
                {
                    // Block white spearman's movement and some attacks
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Wall; // Cannot move through wall

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Archer | Figure.IsWhite; // Friendly - cannot attack

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Peasant | Figure.IsBlack; // Can attack enemy

                    var upLeft = src.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));
                    if (upLeft != -1) b[upLeft] = Figure.Knight | Figure.IsBlack; // Can attack enemy
                },
                src,
                whiteSpearman,
                Spearman.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("White - Edge case H1", _ => { /* setup below places spearman at edge via src override */ },
                7, // h1
                whiteSpearman,
                Spearman.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Black - Edge case A8", _ => { /* setup below places spearman at edge via src override */ },
                56, // a8
                blackSpearman,
                Spearman.GetPossibleActions
            )
        });
    }
}