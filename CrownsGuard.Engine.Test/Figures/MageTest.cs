using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class MageTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Verify()
    {
        const int src = 27; // d4 (3,3)
        const Figure mage = Figure.Mage | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ => { /* empty board around */ },
            src,
            mage,
            Mage.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_BlockedTargets_Verify()
    {
        const int src = 27; // d4 (3,3)
        const Figure mage = Figure.Mage | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // Block some of the 8 target tiles at distance 2
                // Targets: (1,1), (1,3), (1,5), (3,1), (3,5), (5,1), (5,3), (5,5)
                var t11 = 1 * 8 + 1; // (1,1)
                var t15 = 1 * 8 + 5; // (1,5)
                var t31 = 3 * 8 + 1; // (3,1)
                var t55 = 5 * 8 + 5; // (5,5)

                b[t11] = Figure.Peasant | Figure.IsWhite; // friendly blocks
                b[t15] = Figure.Peasant | Figure.IsBlack; // enemy blocks (not walkable)
                b[t31] = Figure.Wall; // wall blocks
                b[t55] = Figure.Knight | Figure.IsBlack; // enemy blocks
            },
            src,
            mage,
            Mage.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_Verify()
    {
        const int src = 0; // a1 (0,0)
        const Figure mage = Figure.Mage | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ => { /* at corner, only in-bounds moves */ },
            src,
            mage,
            Mage.GetPossibleActions
        ));
    }
}
