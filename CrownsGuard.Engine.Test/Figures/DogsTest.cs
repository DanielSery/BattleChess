using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class DogsTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure dogs = Figure.Dogs | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                dogs,
                Dogs.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario(
                "Mixed", b =>
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
            ),
            
            TestUtils.RunGetActionsScenario("Edge case a1", _ => { },
                0,
                Figure.Dogs | Figure.IsWhite,
                Dogs.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario("Edge case h8", _ => { },
                63,
                Figure.Dogs | Figure.IsBlack,
                Dogs.GetPossibleActions
            )
        });
    }

    [Fact]
    public Task ExecuteMeleeAttack_White_Verify()
    {
        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario(
                "Execute up melee", b =>
                {
                    var dst = 27.GetWithOffset(PositionConstants.U);
                    if (dst != -1) b[dst] = Figure.Knight | Figure.IsBlack;
                },
                27,
                Figure.Dogs | Figure.IsWhite,
                PositionConstants.U,
                FigureActionType.MeleeAttack
            )
        });
    }
}
