using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class MountedKnightTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Verify()
    {
        const int src = 27; // d4
        const Figure mountedKnight = Figure.MountedKnight | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                mountedKnight,
                MountedKnight.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Clear paths", b =>
                {
                    // Clear vertical path up
                    for (var i = 1; i <= 3; i++)
                    {
                        var idx = src.GetWithOffset((short)(0 + i * PositionConstants.YOffset));
                        if (idx != -1) b[idx] = Figure.Empty;
                    }

                    // Clear horizontal path right
                    for (var i = 1; i <= 3; i++)
                    {
                        var idx = src.GetWithOffset((short)(i + 0 * PositionConstants.YOffset));
                        if (idx != -1) b[idx] = Figure.Empty;
                    }
                },
                src,
                mountedKnight,
                MountedKnight.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Blocked paths", b =>
                {
                    // Block vertical path up with friendly piece
                    var up1 = src.GetWithOffset(PositionConstants.U1);
                    if (up1 != -1) b[up1] = Figure.Peasant | Figure.IsWhite;

                    // Block horizontal path right with enemy
                    var right1 = src.GetWithOffset(PositionConstants.R1);
                    if (right1 != -1) b[right1] = Figure.Archer | Figure.IsBlack;

                    // Clear path down
                    for (var i = 1; i <= 3; i++)
                    {
                        var idx = src.GetWithOffset((short)(0 - i * PositionConstants.YOffset));
                        if (idx != -1) b[idx] = Figure.Empty;
                    }
                },
                src,
                mountedKnight,
                MountedKnight.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Enemy in range", b =>
                {
                    // Place enemy at distance 2 up
                    var up2 = src.GetWithOffset(PositionConstants.U2);
                    if (up2 != -1) b[up2] = Figure.Knight | Figure.IsBlack;

                    // Place enemy at distance 3 right
                    var right3 = src.GetWithOffset(PositionConstants.U3);
                    if (right3 != -1) b[right3] = Figure.Mage | Figure.IsBlack;
                },
                src,
                mountedKnight,
                MountedKnight.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case A1", _ => { /* setup below places mounted knight at edge via src override */ },
                0, // a1
                mountedKnight,
                MountedKnight.GetPossibleActions
            )
        });
    }
}