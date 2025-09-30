using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class PikemanTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure pikeman = Figure.Pikeman | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                pikeman,
                Pikeman.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Movement in all directions", b =>
                {
                    // Clear adjacent squares for movement
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Empty;

                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Empty;

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Empty;

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Empty;
                },
                src,
                pikeman,
                Pikeman.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Diagonal attacks", b =>
                {
                    // Setup diagonal attack positions (same for both colors)
                    var upLeft = src.GetWithOffset(PositionConstants.U1L1);
                    if (upLeft != -1) b[upLeft] = Figure.Peasant | Figure.IsBlack; // Can attack enemy

                    var upRight = src.GetWithOffset(PositionConstants.U1R1);
                    if (upRight != -1) b[upRight] = Figure.Archer | Figure.IsBlack; // Can attack enemy

                    var downLeft = src.GetWithOffset(PositionConstants.D1L1);
                    if (downLeft != -1) b[downLeft] = Figure.Knight | Figure.IsBlack; // Can attack enemy

                    var downRight = src.GetWithOffset(PositionConstants.D1R1);
                    if (downRight != -1) b[downRight] = Figure.Mage | Figure.IsBlack; // Can attack enemy
                },
                src,
                pikeman,
                Pikeman.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Blocked movement and attacks", b =>
                {
                    // Block some movements
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Wall; // Cannot move through wall

                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Archer | Figure.IsWhite; // Friendly blocks movement

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Empty; // Can move here

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Peasant | Figure.IsBlack; // Enemy blocks movement but can attack

                    // Block some attacks
                    var upLeft = src.GetWithOffset(PositionConstants.U1L1);
                    if (upLeft != -1) b[upLeft] = Figure.Builder | Figure.IsWhite; // Friendly - cannot attack

                    var upRight = src.GetWithOffset(PositionConstants.U1R1);
                    if (upRight != -1) b[upRight] = Figure.Knight | Figure.IsBlack; // Can attack enemy
                },
                src,
                pikeman,
                Pikeman.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case A1", _ => { /* setup below places pikeman at edge via src override */ },
                0, // a1
                pikeman,
                Pikeman.GetPossibleActions
            )
        });
    }
}