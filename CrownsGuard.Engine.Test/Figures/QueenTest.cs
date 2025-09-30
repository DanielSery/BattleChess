using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class QueenTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure queen = Figure.Queen | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                queen,
                Queen.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Clear paths in all directions", b =>
                {
                    // Clear all eight directions for movement
                    var directions = new[] {
                        PositionConstants.U1, PositionConstants.D1, PositionConstants.L1, PositionConstants.R1,
                        PositionConstants.U1L1, PositionConstants.U1R1, PositionConstants.D1L1, PositionConstants.D1R1
                    };

                    foreach (var dir in directions)
                    {
                        for (var i = 1; i <= 3; i++)
                        {
                            var targetIndex = src;
                            for (var j = 0; j < i; j++)
                            {
                                targetIndex = targetIndex.GetWithOffset((short)dir);
                                if (targetIndex == -1) break;
                            }
                            if (targetIndex != -1 && targetIndex != src) b[targetIndex] = Figure.Empty;
                        }
                    }
                },
                src,
                queen,
                Queen.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Attacks in all directions", b =>
                {
                    // Place enemies in all eight directions at various distances
                    var up1 = src.GetWithOffset(PositionConstants.U1);
                    if (up1 != -1) b[up1] = Figure.Peasant | Figure.IsBlack; // Enemy at distance 1

                    var down2 = src.GetWithOffset((short)(0 - 2 * PositionConstants.YOffset));
                    if (down2 != -1) b[down2] = Figure.Archer | Figure.IsBlack; // Enemy at distance 2

                    var left3 = src.GetWithOffset((short)(-3 + 0 * PositionConstants.YOffset));
                    if (left3 != -1) b[left3] = Figure.Knight | Figure.IsBlack; // Enemy at distance 3

                    var right1 = src.GetWithOffset(PositionConstants.R1);
                    if (right1 != -1) b[right1] = Figure.Mage | Figure.IsBlack; // Enemy at distance 1

                    var ul2 = src.GetWithOffset((short)(-2 + -2 * PositionConstants.YOffset));
                    if (ul2 != -1) b[ul2] = Figure.Builder | Figure.IsBlack; // Enemy at distance 2

                    var ur1 = src.GetWithOffset(PositionConstants.U1R1);
                    if (ur1 != -1) b[ur1] = Figure.Dragon | Figure.IsBlack; // Enemy at distance 1

                    var dl3 = src.GetWithOffset((short)(-3 + 3 * PositionConstants.YOffset));
                    if (dl3 != -1) b[dl3] = Figure.Barbarian | Figure.IsBlack; // Enemy at distance 3

                    var dr2 = src.GetWithOffset((short)(2 + 2 * PositionConstants.YOffset));
                    if (dr2 != -1) b[dr2] = Figure.Wizzard | Figure.IsBlack; // Enemy at distance 2
                },
                src,
                queen,
                Queen.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Blocked paths", b =>
                {
                    // Block some directions with obstacles
                    var up1 = src.GetWithOffset(PositionConstants.U1);
                    if (up1 != -1) b[up1] = Figure.Wall; // Wall blocks up

                    var down1 = src.GetWithOffset(PositionConstants.D1);
                    if (down1 != -1) b[down1] = Figure.Archer | Figure.IsWhite; // Friendly blocks down

                    var left1 = src.GetWithOffset(PositionConstants.L1);
                    if (left1 != -1) b[left1] = Figure.Empty; // Clear for movement

                    var right2 = src.GetWithOffset((short)(2 + 0 * PositionConstants.YOffset));
                    if (right2 != -1) b[right2] = Figure.Knight | Figure.IsBlack; // Enemy at distance 2

                    var ul1 = src.GetWithOffset(PositionConstants.U1L1);
                    if (ul1 != -1) b[ul1] = Figure.Peasant | Figure.IsWhite; // Friendly blocks diagonal

                    var ur2 = src.GetWithOffset((short)(2 + -2 * PositionConstants.YOffset));
                    if (ur2 != -1) b[ur2] = Figure.Empty; // Clear diagonal movement

                    var dl1 = src.GetWithOffset(PositionConstants.D1L1);
                    if (dl1 != -1) b[dl1] = Figure.Mage | Figure.IsBlack; // Enemy blocks diagonal

                    var dr3 = src.GetWithOffset((short)(3 + 3 * PositionConstants.YOffset));
                    if (dr3 != -1) b[dr3] = Figure.Builder | Figure.IsBlack; // Enemy at distance 3
                },
                src,
                queen,
                Queen.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case A1", _ => { /* setup below places queen at edge via src override */ },
                0, // a1
                queen,
                Queen.GetPossibleActions
            )
        });
    }
}