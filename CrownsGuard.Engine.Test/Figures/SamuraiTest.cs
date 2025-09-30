using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class SamuraiTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure samurai = Figure.Samurai | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                samurai,
                Samurai.GetPossibleActions
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
                samurai,
                Samurai.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Piercing attacks", b =>
                {
                    // Setup piercing attacks in diagonal directions
                    // UL direction: enemy at distance 2
                    var ul1 = src.GetWithOffset(PositionConstants.U1L1);
                    var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.U1L1);
                    var ul3 = ul2 == -1 ? -1 : ul2.GetWithOffset(PositionConstants.U1L1);

                    if (ul1 != -1) b[ul1] = Figure.Empty;
                    if (ul2 != -1) b[ul2] = Figure.Empty;
                    if (ul3 != -1) b[ul3] = Figure.Peasant | Figure.IsBlack; // Enemy at distance 3

                    // UR direction: enemy at distance 1
                    var ur1 = src.GetWithOffset(PositionConstants.U1R1);
                    if (ur1 != -1) b[ur1] = Figure.Archer | Figure.IsBlack; // Enemy at distance 1

                    // DL direction: blocked at distance 2
                    var dl1 = src.GetWithOffset(PositionConstants.D1L1);
                    var dl2 = dl1 == -1 ? -1 : dl1.GetWithOffset(PositionConstants.D1L1);

                    if (dl1 != -1) b[dl1] = Figure.Empty;
                    if (dl2 != -1) b[dl2] = Figure.Knight | Figure.IsWhite; // Friendly blocks

                    // DR direction: clear path
                    var dr1 = src.GetWithOffset(PositionConstants.D1R1);
                    var dr2 = dr1 == -1 ? -1 : dr1.GetWithOffset(PositionConstants.D1R1);
                    var dr3 = dr2 == -1 ? -1 : dr2.GetWithOffset(PositionConstants.D1R1);

                    if (dr1 != -1) b[dr1] = Figure.Empty;
                    if (dr2 != -1) b[dr2] = Figure.Empty;
                    if (dr3 != -1) b[dr3] = Figure.Empty; // No enemy, but shows possible attack
                },
                src,
                samurai,
                Samurai.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case A1", _ => { /* setup below places samurai at edge via src override */ },
                0, // a1
                samurai,
                Samurai.GetPossibleActions
            )
        });
    }
}