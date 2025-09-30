using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class PriestTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure priest = Figure.Priest | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                priest,
                Priest.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Diagonal movement", b =>
                {
                    // Clear diagonal paths for movement
                    var ul1 = src.GetWithOffset(PositionConstants.U1L1);
                    var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.U1L1);
                    if (ul1 != -1) b[ul1] = Figure.Empty;
                    if (ul2 != -1) b[ul2] = Figure.Empty;

                    var ur1 = src.GetWithOffset(PositionConstants.U1R1);
                    var ur2 = ur1 == -1 ? -1 : ur1.GetWithOffset(PositionConstants.U1R1);
                    if (ur1 != -1) b[ur1] = Figure.Empty;
                    if (ur2 != -1) b[ur2] = Figure.Empty;

                    var dl1 = src.GetWithOffset(PositionConstants.D1L1);
                    var dl2 = dl1 == -1 ? -1 : dl1.GetWithOffset(PositionConstants.D1L1);
                    if (dl1 != -1) b[dl1] = Figure.Empty;
                    if (dl2 != -1) b[dl2] = Figure.Empty;

                    var dr1 = src.GetWithOffset(PositionConstants.D1R1);
                    var dr2 = dr1 == -1 ? -1 : dr1.GetWithOffset(PositionConstants.D1R1);
                    if (dr1 != -1) b[dr1] = Figure.Empty;
                    if (dr2 != -1) b[dr2] = Figure.Empty;
                },
                src,
                priest,
                Priest.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Diagonal attacks and MakeUnitKing", b =>
                {
                    // Setup diagonal attacks
                    var ul1 = src.GetWithOffset(PositionConstants.U1L1);
                    var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.U1L1);
                    if (ul1 != -1) b[ul1] = Figure.Empty;
                    if (ul2 != -1) b[ul2] = Figure.Peasant | Figure.IsBlack; // Enemy to attack

                    var ur1 = src.GetWithOffset(PositionConstants.U1R1);
                    if (ur1 != -1) b[ur1] = Figure.Archer | Figure.IsBlack; // Enemy to attack

                    // Setup MakeUnitKing actions on adjacent units
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Knight | Figure.IsWhite; // Ally for MakeUnitKing

                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Mage | Figure.IsBlack; // Enemy for MakeUnitKing

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Builder | Figure.IsWhite; // Ally for MakeUnitKing

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Dragon | Figure.IsBlack; // Enemy for MakeUnitKing
                },
                src,
                priest,
                Priest.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Blocked diagonals", b =>
                {
                    // Block diagonal movement/attack with obstacles
                    var ul1 = src.GetWithOffset(PositionConstants.U1L1);
                    if (ul1 != -1) b[ul1] = Figure.Wall; // Wall blocks

                    var ur1 = src.GetWithOffset(PositionConstants.U1R1);
                    if (ur1 != -1) b[ur1] = Figure.Archer | Figure.IsWhite; // Friendly blocks

                    var dl1 = src.GetWithOffset(PositionConstants.D1L1);
                    if (dl1 != -1) b[dl1] = Figure.Empty; // Clear for movement

                    var dr1 = src.GetWithOffset(PositionConstants.D1R1);
                    var dr2 = dr1 == -1 ? -1 : dr1.GetWithOffset(PositionConstants.D1R1);
                    if (dr1 != -1) b[dr1] = Figure.Empty;
                    if (dr2 != -1) b[dr2] = Figure.Knight | Figure.IsBlack; // Enemy at distance 2
                },
                src,
                priest,
                Priest.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case", _ => { /* setup below places priest at edge via src override */ },
                0, // a1
                priest,
                Priest.GetPossibleActions
            )
        });
    }
}