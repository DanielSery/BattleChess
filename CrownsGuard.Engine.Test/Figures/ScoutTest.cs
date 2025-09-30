using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class ScoutTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure scout = Figure.Scout | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                scout,
                Scout.GetPossibleActions
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
                scout,
                Scout.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Adjacent attacks", b =>
                {
                    // Place enemies adjacent in all eight directions
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
                },
                src,
                scout,
                Scout.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case", _ => { /* setup below places scout at edge via src override */ },
                0, // a1
                scout,
                Scout.GetPossibleActions
            )
        });
    }
}