using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class NordguardTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure nordguard = Figure.Nordguard | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                nordguard,
                Nordguard.GetPossibleActions
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
                nordguard,
                Nordguard.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Piercing attacks", b =>
                {
                    // Setup piercing attacks in rook directions
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
                },
                src,
                nordguard,
                Nordguard.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Blocked diagonals", b =>
                {
                    // Block diagonal movement with various obstacles
                    var ul = src.GetWithOffset(PositionConstants.U1L1);
                    if (ul != -1) b[ul] = Figure.Wall; // Wall blocks

                    var ur = src.GetWithOffset(PositionConstants.U1R1);
                    if (ur != -1) b[ur] = Figure.Peasant | Figure.IsBlack; // Enemy blocks

                    var dl = src.GetWithOffset(PositionConstants.D1L1);
                    if (dl != -1) b[dl] = Figure.Archer | Figure.IsWhite; // Friendly blocks

                    var dr = src.GetWithOffset(PositionConstants.D1R1);
                    if (dr != -1) b[dr] = Figure.Empty; // Clear for movement
                },
                src,
                nordguard,
                Nordguard.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case", _ => { /* setup below places nordguard at edge via src override */ },
                0, // a1
                nordguard,
                Nordguard.GetPossibleActions
            )
        });
    }
}