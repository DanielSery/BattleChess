using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class KnightTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure knight = Figure.Knight | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty around */ }, src, knight, Knight.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Mixed blocking and attacks", b =>
                {
                    // Left: empty then friendly blocker at distance 2
                    var l1 = src.GetWithOffset(PositionConstants.L1);
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L1);
                    if (l1 != -1) b[l1] = Figure.Empty; // walkable
                    if (l2 != -1) b[l2] = Figure.Peasant | Figure.IsWhite; // friendly blocks

                    // Up: enemy at distance 3
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Empty; // walkable
                    if (u2 != -1) b[u2] = Figure.Empty; // walkable
                    if (u3 != -1) b[u3] = Figure.Peasant | Figure.IsBlack; // enemy to strike

                    // Right: wall immediately
                    var r1 = src.GetWithOffset(PositionConstants.R1);
                    if (r1 != -1) b[r1] = Figure.Wall; // non-walkable blocker

                    // Down: friendly immediately
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    if (d1 != -1) b[d1] = Figure.LegionarySword | Figure.IsWhite; // non-walkable blocker
                }, src, knight, Knight.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                _ => { },
                0, // a1
                knight,
                Knight.GetPossibleActions)
        });
    }

    [Fact]
    public Task ExecuteMeleePierceAttack_Verify()
    {
        const int src = 27; // d4
        const Figure whiteKnight = Figure.Knight | Figure.IsWhite;
        const Figure blackKnight = Figure.Knight | Figure.IsBlack;

        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario("White knight attacks enemy at (2,1) distance - up right", b =>
                {
                    // Place enemy at 2 right, 1 up from knight (U2R1)
                    var target = src.GetWithOffset(PositionConstants.U2R1);
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsBlack; // enemy to attack
                },
                src, whiteKnight,
                PositionConstants.U2R1,
                FigureActionType.MeleePierceAttack
            ),

            TestUtils.RunExecuteActionScenario("White knight attacks enemy at (2,1) distance - up left", b =>
                {
                    // Place enemy at 2 left, 1 up from knight (U2L1)
                    var target = src.GetWithOffset(PositionConstants.U2L1);
                    if (target != -1) b[target] = Figure.Archer | Figure.IsBlack; // enemy to attack
                },
                src, whiteKnight,
                PositionConstants.U2L1,
                FigureActionType.MeleePierceAttack
            ),

            TestUtils.RunExecuteActionScenario("White knight attacks enemy at (1,2) distance - up right", b =>
                {
                    // Place enemy at 1 right, 2 up from knight (U1R2)
                    var target = src.GetWithOffset(PositionConstants.U1R2);
                    if (target != -1) b[target] = Figure.Mage | Figure.IsBlack; // enemy to attack
                },
                src, whiteKnight,
                PositionConstants.U1R2,
                FigureActionType.MeleePierceAttack
            ),

            TestUtils.RunExecuteActionScenario("White knight attacks enemy at (1,2) distance - up left", b =>
                {
                    // Place enemy at 1 left, 2 up from knight (U1L2)
                    var target = src.GetWithOffset(PositionConstants.U1L2);
                    if (target != -1) b[target] = Figure.Trader | Figure.IsBlack; // enemy to attack
                },
                src, whiteKnight,
                PositionConstants.U1L2,
                FigureActionType.MeleePierceAttack
            ),

            TestUtils.RunExecuteActionScenario("Black knight attacks enemy at (2,1) distance - down right", b =>
                {
                    // For black knight, forward is down. Place enemy at 2 right, 1 down (D2R1)
                    var target = src.GetWithOffset(PositionConstants.D2R1);
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsWhite; // enemy to attack
                },
                src, blackKnight,
                PositionConstants.D2R1,
                FigureActionType.MeleePierceAttack
            ),

            TestUtils.RunExecuteActionScenario("Black knight attacks enemy at (2,1) distance - down left", b =>
                {
                    // For black knight, forward is down. Place enemy at 2 left, 1 down (D2L1)
                    var target = src.GetWithOffset(PositionConstants.D2L1);
                    if (target != -1) b[target] = Figure.Archer | Figure.IsWhite; // enemy to attack
                },
                src, blackKnight,
                PositionConstants.D2L1,
                FigureActionType.MeleePierceAttack
            ),

            TestUtils.RunExecuteActionScenario("Black knight attacks enemy at (1,2) distance - down right", b =>
                {
                    // For black knight, forward is down. Place enemy at 1 right, 2 down (D1R2)
                    var target = src.GetWithOffset(PositionConstants.D1R2);
                    if (target != -1) b[target] = Figure.Mage | Figure.IsWhite; // enemy to attack
                },
                src, blackKnight,
                PositionConstants.D1R2,
                FigureActionType.MeleePierceAttack
            ),

            TestUtils.RunExecuteActionScenario("Black knight attacks enemy at (1,2) distance - down left", b =>
                {
                    // For black knight, forward is down. Place enemy at 1 left, 2 down (D1L2)
                    var target = src.GetWithOffset(PositionConstants.D1L2);
                    if (target != -1) b[target] = Figure.Trader | Figure.IsWhite; // enemy to attack
                },
                src, blackKnight,
                PositionConstants.D1L2,
                FigureActionType.MeleePierceAttack
            )
        });
    }
}
