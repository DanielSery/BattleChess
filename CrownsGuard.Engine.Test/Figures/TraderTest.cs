using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class TraderTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure trader = Figure.Trader | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                trader,
                Trader.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Movement and attacks", b =>
                {
                    // Place enemies adjacent in all directions for attacks
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Peasant | Figure.IsBlack;

                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Archer | Figure.IsBlack;

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Knight | Figure.IsBlack;

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Mage | Figure.IsBlack;

                    var ul = src.GetWithOffset(PositionConstants.U1L1);
                    if (ul != -1) b[ul] = Figure.Builder | Figure.IsBlack;

                    var ur = src.GetWithOffset(PositionConstants.U1R1);
                    if (ur != -1) b[ur] = Figure.Dragon | Figure.IsBlack;

                    var dl = src.GetWithOffset(PositionConstants.D1L1);
                    if (dl != -1) b[dl] = Figure.Barbarian | Figure.IsBlack;

                    var dr = src.GetWithOffset(PositionConstants.D1R1);
                    if (dr != -1) b[dr] = Figure.Wizzard | Figure.IsBlack;

                    // Place some allies for swapping (at various positions)
                    b[0] = Figure.Peasant | Figure.IsWhite; // a1
                    b[7] = Figure.Archer | Figure.IsWhite; // h1
                    b[56] = Figure.Knight | Figure.IsWhite; // a8
                    b[63] = Figure.Mage | Figure.IsWhite; // h8
                },
                src,
                trader,
                Trader.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Swap with allies only", b =>
                {
                    // Place mix of allies and enemies
                    b[0] = Figure.Peasant | Figure.IsWhite; // Ally for swap
                    b[7] = Figure.Archer | Figure.IsBlack; // Enemy - no swap
                    b[56] = Figure.Knight | Figure.IsWhite; // Ally for swap
                    b[63] = Figure.Mage | Figure.IsBlack; // Enemy - no swap

                    // Place adjacent enemy for attack
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Builder | Figure.IsBlack; // Enemy for attack
                },
                src,
                trader,
                Trader.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case A1", _ => { /* setup below places trader at edge via src override */ },
                0, // a1
                trader,
                Trader.GetPossibleActions
            )
        });
    }
}