using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class MusketeerTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure whiteMusketeer = Figure.Musketeer | Figure.IsWhite;
        const Figure blackMusketeer = Figure.Musketeer | Figure.IsBlack;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "White - All empty", _ => { /* empty around */ },
                src,
                whiteMusketeer,
                Musketeer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - All empty", _ => { /* empty around */ },
                src,
                blackMusketeer,
                Musketeer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "White - Enemy adjacent suppresses", b =>
                {
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Peasant | Figure.IsBlack; // Adjacent enemy suppresses
                },
                src,
                whiteMusketeer,
                Musketeer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - Enemy adjacent suppresses", b =>
                {
                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Archer | Figure.IsWhite; // Adjacent enemy suppresses
                },
                src,
                blackMusketeer,
                Musketeer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "White - Clear attack lines", b =>
                {
                    // Clear backward attack lines (white attacks backward)
                    var leftBack1 = src.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));
                    var leftBack2 = leftBack1 == -1 ? -1 : leftBack1.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));
                    var leftBack3 = leftBack2 == -1 ? -1 : leftBack2.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));

                    if (leftBack1 != -1) b[leftBack1] = Figure.Empty;
                    if (leftBack2 != -1) b[leftBack2] = Figure.Empty;
                    if (leftBack3 != -1) b[leftBack3] = Figure.Peasant | Figure.IsBlack; // Enemy at distance 3

                    var rightBack1 = src.GetWithOffset((short)(1 + -1 * PositionConstants.YOffset));
                    var rightBack2 = rightBack1 == -1 ? -1 : rightBack1.GetWithOffset((short)(1 + -1 * PositionConstants.YOffset));
                    var rightBack3 = rightBack2 == -1 ? -1 : rightBack2.GetWithOffset((short)(1 + -1 * PositionConstants.YOffset));

                    if (rightBack1 != -1) b[rightBack1] = Figure.Empty;
                    if (rightBack2 != -1) b[rightBack2] = Figure.Empty;
                    if (rightBack3 != -1) b[rightBack3] = Figure.Knight | Figure.IsBlack; // Enemy at distance 3
                },
                src,
                whiteMusketeer,
                Musketeer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - Clear attack lines", b =>
                {
                    // Clear forward attack lines (black attacks forward)
                    var leftForward1 = src.GetWithOffset((short)(-1 + 1 * PositionConstants.YOffset));
                    var leftForward2 = leftForward1 == -1 ? -1 : leftForward1.GetWithOffset((short)(-1 + 1 * PositionConstants.YOffset));
                    var leftForward3 = leftForward2 == -1 ? -1 : leftForward2.GetWithOffset((short)(-1 + 1 * PositionConstants.YOffset));

                    if (leftForward1 != -1) b[leftForward1] = Figure.Empty;
                    if (leftForward2 != -1) b[leftForward2] = Figure.Empty;
                    if (leftForward3 != -1) b[leftForward3] = Figure.Mage | Figure.IsWhite; // Enemy at distance 3

                    var rightForward1 = src.GetWithOffset((short)(1 + 1 * PositionConstants.YOffset));
                    var rightForward2 = rightForward1 == -1 ? -1 : rightForward1.GetWithOffset((short)(1 + 1 * PositionConstants.YOffset));
                    var rightForward3 = rightForward2 == -1 ? -1 : rightForward2.GetWithOffset((short)(1 + 1 * PositionConstants.YOffset));

                    if (rightForward1 != -1) b[rightForward1] = Figure.Empty;
                    if (rightForward2 != -1) b[rightForward2] = Figure.Empty;
                    if (rightForward3 != -1) b[rightForward3] = Figure.Builder | Figure.IsWhite; // Enemy at distance 3
                },
                src,
                blackMusketeer,
                Musketeer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("White - Edge case", _ => { /* setup below places musketeer at edge via src override */ },
                63, // h8
                whiteMusketeer,
                Musketeer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Black - Edge case", _ => { /* setup below places musketeer at edge via src override */ },
                0, // a1
                blackMusketeer,
                Musketeer.GetPossibleActions
            )
        });
    }
}