using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class BarbarianTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Verify()
    {
        const int src = 27; // d4
        const Figure barbarian = Figure.Barbarian | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario("All empty", _ => { /* empty around */ },
                src,
                barbarian,
                Barbarian.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario(
                "Push line left", b =>
                {
                    var l1 = src.GetWithOffset(PositionConstants.L);
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L);
                    var l3 = l2 == -1 ? -1 : l2.GetWithOffset(PositionConstants.L);
                    var l4 = l3 == -1 ? -1 : l3.GetWithOffset(PositionConstants.L);
                    if (l1 != -1) b[l1] = Figure.LegionarySword | Figure.IsBlack; // adjacent non-empty (enemy)
                    if (l2 != -1) b[l2] = Figure.Empty; // empty target -> PushFigure
                    if (l3 != -1) b[l3] = Figure.Empty; // empty target -> PushFigure
                    if (l4 != -1) b[l4] = Figure.Wall;  // blocker -> PossiblePushFigure at l4 and beyond
                },
                src,
                barbarian,
                Barbarian.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario(
                "Push line up", b =>
                {
                    var u1 = src.GetWithOffset(PositionConstants.U);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U);
                    if (u1 != -1) b[u1] = Figure.Peasant | Figure.IsWhite; // adjacent non-empty (friendly)
                    if (u2 != -1) b[u2] = Figure.LegionarySword | Figure.IsWhite; // non-walkable blocker right after -> PossiblePushFigure from u2
                    if (u3 != -1) b[u3] = Figure.Empty; // will still appear as PossiblePushFigure in current behavior
                },
                src,
                barbarian,
                Barbarian.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario(
                "Mixed directions", b =>
                {
                    // Right: adjacent enemy then empties
                    var r1 = src.GetWithOffset(PositionConstants.R);
                    var r2 = r1 == -1 ? -1 : r1.GetWithOffset(PositionConstants.R);
                    var r3 = r2 == -1 ? -1 : r2.GetWithOffset(PositionConstants.R);
                    if (r1 != -1) b[r1] = Figure.Trader | Figure.IsBlack;
                    if (r2 != -1) b[r2] = Figure.Empty;
                    if (r3 != -1) b[r3] = Figure.Empty;

                    // Down: adjacent friendly then blocker immediately
                    var d1 = src.GetWithOffset(PositionConstants.D);
                    var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D);
                    if (d1 != -1) b[d1] = Figure.Archer | Figure.IsWhite;
                    if (d2 != -1) b[d2] = Figure.Wall;

                    // Down-Left: adjacent enemy then empty then blocker
                    var dl1 = src.GetWithOffset(PositionConstants.DL);
                    var dl2 = dl1 == -1 ? -1 : dl1.GetWithOffset(PositionConstants.DL);
                    var dl3 = dl2 == -1 ? -1 : dl2.GetWithOffset(PositionConstants.DL);
                    if (dl1 != -1) b[dl1] = Figure.Knight | Figure.IsBlack;
                    if (dl2 != -1) b[dl2] = Figure.Empty;
                    if (dl3 != -1) b[dl3] = Figure.Knight | Figure.IsBlack;
                },
                src,
                barbarian,
                Barbarian.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario("Edge case", _ => { /* no additional setup */ },
                0, // a1
                barbarian,
                Barbarian.GetPossibleActions
            )
        });
    }
}
