using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class DogsTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_White_Verify()
    {
        const int src = 27; // d4
        const Figure dogs = Figure.Dogs | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ => { /* empty around */ },
            src,
            dogs,
            Dogs.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_Mixed_White_Verify()
    {
        const int src = 27; // d4
        const Figure dogs = Figure.Dogs | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // UL: enemy at distance 2 (should add Possible at 1 and MeleeAttack at 2)
                var ul1 = src.GetWithOffset(PositionConstants.UL);
                var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.UL);
                if (ul2 != -1) b[ul2] = Figure.Peasant | Figure.IsBlack;

                // R: friendly at distance 1 (should add PossibleMeleeAttack at R1 and break)
                var r1 = src.GetWithOffset(PositionConstants.R);
                if (r1 != -1) b[r1] = Figure.LegionarySword | Figure.IsWhite;

                // D: wall at distance 3 (should add PossibleMeleeAttack at D1, D2 and D3 then break)
                var d1 = src.GetWithOffset(PositionConstants.D);
                var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D);
                var d3 = d2 == -1 ? -1 : d2.GetWithOffset(PositionConstants.D);
                if (d3 != -1) b[d3] = Figure.Wall;

                // Keep adjacent walkable squares empty for move generation in other directions
                // Also ensure immediate diagonals other than UL are empty (to avoid unexpected blocking)
            },
            src,
            dogs,
            Dogs.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_White_Verify()
    {
        const int src = 0; // a1
        const Figure dogs = Figure.Dogs | Figure.IsWhite;
        return Verify(TestUtils.RunGetActionsScenario(_ => { },
            src,
            dogs,
            Dogs.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_H8_Black_Verify()
    {
        const int src = 63; // h8
        const Figure dogs = Figure.Dogs | Figure.IsBlack;
        return Verify(TestUtils.RunGetActionsScenario(_ => { },
            src,
            dogs,
            Dogs.GetPossibleActions
        ));
    }

    [Fact]
    public Task ExecuteMeleeAttack_White_Verify()
    {
        const int src = 27; // d4
        const Figure dogs = Figure.Dogs | Figure.IsWhite;

        // choose an adjacent target: Up relative
        short relative = PositionConstants.U;

        return Verify(TestUtils.RunExecuteActionScenario(b =>
            {
                var dst = src.GetWithOffset(relative);
                if (dst != -1) b[dst] = Figure.Knight | Figure.IsBlack;
            },
            src,
            dogs,
            relative,
            FigureActionType.MeleeAttack
        ));
    }
}
