using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class MountedArcherTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure mountedArcher = Figure.MountedArcher | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                mountedArcher,
                MountedArcher.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Enemy diagonal adjacent", b =>
                {
                    var upRight = src.GetWithOffset(PositionConstants.U1R1);
                    if (upRight != -1) b[upRight] = Figure.Peasant | Figure.IsBlack; // enemy diagonal adjacent
                },
                src,
                mountedArcher,
                MountedArcher.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Blocking within movement range", b =>
                {
                    // Left: empty then friendly blocker at distance 2
                    var l1 = src.GetWithOffset(PositionConstants.L1);
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L1);
                    if (l1 != -1) b[l1] = Figure.Empty; // walkable
                    if (l2 != -1) b[l2] = Figure.Peasant | Figure.IsWhite; // friendly blocks

                    // Up: enemy diagonal at distance 1
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var ur1 = src.GetWithOffset(PositionConstants.U1R1);
                    if (u1 != -1) b[u1] = Figure.Empty; // walkable
                    if (ur1 != -1) b[ur1] = Figure.Peasant | Figure.IsBlack; // enemy diagonal

                    // Right: wall immediately
                    var r1 = src.GetWithOffset(PositionConstants.R1);
                    if (r1 != -1) b[r1] = Figure.Wall; // non-walkable blocker

                    // Down: friendly immediately
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    if (d1 != -1) b[d1] = Figure.LegionarySword | Figure.IsWhite; // non-walkable blocker
                },
                src,
                mountedArcher,
                MountedArcher.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case", _ => { /* setup below places mounted archer at edge via src override */ },
                0, // a1
                mountedArcher,
                MountedArcher.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Long rook movement", b =>
                {
                    // Left: clear path of 3 squares
                    var l1 = src.GetWithOffset(PositionConstants.L1);
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L1);
                    var l3 = l2 == -1 ? -1 : l2.GetWithOffset(PositionConstants.L1);
                    if (l1 != -1) b[l1] = Figure.Empty;
                    if (l2 != -1) b[l2] = Figure.Empty;
                    if (l3 != -1) b[l3] = Figure.Empty;

                    // Up: clear path of 2 squares
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Empty;
                    if (u2 != -1) b[u2] = Figure.Empty;

                    // Right: clear path of 4 squares
                    var r1 = src.GetWithOffset(PositionConstants.R1);
                    var r2 = r1 == -1 ? -1 : r1.GetWithOffset(PositionConstants.R1);
                    var r3 = r2 == -1 ? -1 : r2.GetWithOffset(PositionConstants.R1);
                    var r4 = r3 == -1 ? -1 : r3.GetWithOffset(PositionConstants.R1);
                    if (r1 != -1) b[r1] = Figure.Empty;
                    if (r2 != -1) b[r2] = Figure.Empty;
                    if (r3 != -1) b[r3] = Figure.Empty;
                    if (r4 != -1) b[r4] = Figure.Empty;

                    // Down: clear path of 1 square
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    if (d1 != -1) b[d1] = Figure.Empty;
                },
                src,
                mountedArcher,
                MountedArcher.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Diagonal attacks all directions", b =>
                {
                    // Place enemies on all diagonal positions
                    var ul = src.GetWithOffset(PositionConstants.U1L1);
                    var ur = src.GetWithOffset(PositionConstants.U1R1);
                    var dl = src.GetWithOffset(PositionConstants.D1L1);
                    var dr = src.GetWithOffset(PositionConstants.D1R1);

                    if (ul != -1) b[ul] = Figure.Peasant | Figure.IsBlack; // enemy diagonal
                    if (ur != -1) b[ur] = Figure.Knight | Figure.IsBlack; // enemy diagonal
                    if (dl != -1) b[dl] = Figure.Trader | Figure.IsBlack; // enemy diagonal
                    if (dr != -1) b[dr] = Figure.Archer | Figure.IsBlack; // enemy diagonal
                },
                src,
                mountedArcher,
                MountedArcher.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Mixed blocking scenarios", b =>
                {
                    // Left: empty, empty, wall at distance 3
                    var l1 = src.GetWithOffset(PositionConstants.L1);
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L1);
                    var l3 = l2 == -1 ? -1 : l2.GetWithOffset(PositionConstants.L1);
                    if (l1 != -1) b[l1] = Figure.Empty;
                    if (l2 != -1) b[l2] = Figure.Empty;
                    if (l3 != -1) b[l3] = Figure.Wall;

                    // Up-Right diagonal: friendly at distance 1
                    var ur1 = src.GetWithOffset(PositionConstants.U1R1);
                    if (ur1 != -1) b[ur1] = Figure.LegionarySword | Figure.IsWhite;

                    // Down-Left diagonal: enemy at distance 1
                    var dl1 = src.GetWithOffset(PositionConstants.D1L1);
                    if (dl1 != -1) b[dl1] = Figure.Peasant | Figure.IsBlack;

                    // Down-Right diagonal: empty (should create PossibleMeleeAttack)
                    var dr1 = src.GetWithOffset(PositionConstants.D1R1);
                    if (dr1 != -1) b[dr1] = Figure.Empty;

                    // Up-Left diagonal: wall (should create MeleeDefend)
                    var ul1 = src.GetWithOffset(PositionConstants.U1L1);
                    if (ul1 != -1) b[ul1] = Figure.Wall;
                },
                src,
                mountedArcher,
                MountedArcher.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Corner position", _ => { /* setup below places mounted archer at corner via src override */ },
                63, // h8
                mountedArcher,
                MountedArcher.GetPossibleActions
            )
        });
    }
}