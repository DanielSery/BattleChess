using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class RangerTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure ranger = Figure.Ranger | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                ranger,
                Ranger.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Diagonal movement", b =>
                {
                    // Clear diagonal paths for movement
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
                ranger,
                Ranger.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Ranged attacks", b =>
                {
                    // Setup ranged attacks in rook directions
                    // Up direction: enemy at distance 2
                    var up1 = src.GetWithOffset(PositionConstants.U1);
                    var up2 = up1 == -1 ? -1 : up1.GetWithOffset(PositionConstants.U1);
                    var up3 = up2 == -1 ? -1 : up2.GetWithOffset(PositionConstants.U1);

                    if (up1 != -1) b[up1] = Figure.Empty;
                    if (up2 != -1) b[up2] = Figure.Empty;
                    if (up3 != -1) b[up3] = Figure.Peasant | Figure.IsBlack; // Enemy at distance 3

                    // Right direction: enemy at distance 1
                    var right1 = src.GetWithOffset(PositionConstants.R1);
                    if (right1 != -1) b[right1] = Figure.Archer | Figure.IsBlack; // Enemy at distance 1

                    // Down direction: blocked at distance 2
                    var down1 = src.GetWithOffset(PositionConstants.D1);
                    var down2 = down1 == -1 ? -1 : down1.GetWithOffset(PositionConstants.D1);

                    if (down1 != -1) b[down1] = Figure.Empty;
                    if (down2 != -1) b[down2] = Figure.Knight | Figure.IsWhite; // Friendly blocks

                    // Left direction: clear path
                    var left1 = src.GetWithOffset(PositionConstants.L1);
                    var left2 = left1 == -1 ? -1 : left1.GetWithOffset(PositionConstants.L1);
                    var left3 = left2 == -1 ? -1 : left2.GetWithOffset(PositionConstants.L1);

                    if (left1 != -1) b[left1] = Figure.Empty;
                    if (left2 != -1) b[left2] = Figure.Empty;
                    if (left3 != -1) b[left3] = Figure.Empty; // No enemy, but shows possible attack
                },
                src,
                ranger,
                Ranger.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Suppressed by adjacent enemy", b =>
                {
                    // Place enemy adjacent - should suppress ranged attacks
                    var adjacent = src.GetWithOffset(PositionConstants.U1);
                    if (adjacent != -1) b[adjacent] = Figure.Mage | Figure.IsBlack; // Adjacent enemy suppresses

                    // Even though paths are clear, no ranged attacks should be possible
                    var right1 = src.GetWithOffset(PositionConstants.R1);
                    var right2 = right1 == -1 ? -1 : right1.GetWithOffset(PositionConstants.R1);
                    var right3 = right2 == -1 ? -1 : right2.GetWithOffset(PositionConstants.R1);

                    if (right1 != -1) b[right1] = Figure.Empty;
                    if (right2 != -1) b[right2] = Figure.Empty;
                    if (right3 != -1) b[right3] = Figure.Peasant | Figure.IsBlack; // Enemy that can't be attacked due to suppression
                },
                src,
                ranger,
                Ranger.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case A1", _ => { /* setup below places ranger at edge via src override */ },
                0, // a1
                ranger,
                Ranger.GetPossibleActions
            )
        });
    }
}