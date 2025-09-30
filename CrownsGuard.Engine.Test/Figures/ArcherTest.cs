using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class ArcherTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure archer = Figure.Archer | Figure.IsWhite;

        return Verify(new List< object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                archer,
                Archer.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario(
                "Enemy nearby", b =>
                {
                    var upLeft = src.GetWithOffset(PositionConstants.U1L1);
                    if (upLeft != -1) b[upLeft] = Figure.Peasant | Figure.IsBlack; // enemy next to archer
                },
                src,
                archer,
                Archer.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario("Blocking and enemy within 3", b =>
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
                    if (u3 != -1) b[u3] = Figure.Peasant | Figure.IsBlack; // enemy to shoot

                    // Right: wall immediately
                    var r1 = src.GetWithOffset(PositionConstants.R1);
                    if (r1 != -1) b[r1] = Figure.Wall; // non-walkable blocker

                    // Down: friendly immediately
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    if (d1 != -1) b[d1] = Figure.LegionarySword | Figure.IsWhite; // non-walkable blocker
                },
                src,
                archer,
                Archer.GetPossibleActions
            ),
            
            TestUtils.RunGetActionsScenario("Edge case A1 corner", _ => { /* setup below places archer at edge via src override */ },
                0, // a1
                archer,
                Archer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case H1 corner", _ => { /* setup below places archer at edge via src override */ },
                7, // h1
                archer,
                Archer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case A8 corner", _ => { /* setup below places archer at edge via src override */ },
                56, // a8
                archer,
                Archer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case H8 corner", _ => { /* setup below places archer at edge via src override */ },
                63, // h8
                archer,
                Archer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case B1 side edge", _ => { /* setup below places archer at edge via src override */ },
                1, // b1
                archer,
                Archer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case G1 side edge", _ => { /* setup below places archer at edge via src override */ },
                6, // g1
                archer,
                Archer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case A4 side edge", _ => { /* setup below places archer at edge via src override */ },
                24, // a4
                archer,
                Archer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case H4 side edge", _ => { /* setup below places archer at edge via src override */ },
                31, // h4
                archer,
                Archer.GetPossibleActions
            )
        });
    }

    [Fact]
    public Task ExecuteRangedAttack_Verify()
    {
        const int src = 27; // d4
        const Figure archer = Figure.Archer | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario("White attacks enemy at distance 2", b =>
                {
                    var src = 27; // d4
                    // Place enemy at distance 2 up
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Empty; // walkable
                    if (u2 != -1) b[u2] = Figure.Knight | Figure.IsBlack; // enemy to attack
                },
                27, // d4
                archer,
                (short)(PositionConstants.U1 + PositionConstants.U1), // distance 2 up
                FigureActionType.RangedAttack
            ),

            TestUtils.RunExecuteActionScenario("White attacks enemy at distance 3", b =>
                {
                    var src = 27; // d4
                    // Place enemy at distance 3 right
                    var r1 = src.GetWithOffset(PositionConstants.R1);
                    var r2 = r1 == -1 ? -1 : r1.GetWithOffset(PositionConstants.R1);
                    var r3 = r2 == -1 ? -1 : r2.GetWithOffset(PositionConstants.R1);
                    if (r1 != -1) b[r1] = Figure.Empty; // walkable
                    if (r2 != -1) b[r2] = Figure.Empty; // walkable
                    if (r3 != -1) b[r3] = Figure.Peasant | Figure.IsBlack; // enemy to attack
                },
                27, // d4
                archer,
                (short)(PositionConstants.R1 + PositionConstants.R1 + PositionConstants.R1), // distance 3 right
                FigureActionType.RangedAttack
            ),

            TestUtils.RunExecuteActionScenario("Black attacks enemy at distance 2", b =>
                {
                    var src = 36; // e5
                    // For black, forward is down (+YOffset)
                    var d1 = src.GetWithOffset(PositionConstants.D1);
                    var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D1);
                    if (d1 != -1) b[d1] = Figure.Empty; // walkable
                    if (d2 != -1) b[d2] = Figure.Trader | Figure.IsWhite; // enemy to attack
                },
                36, // e5
                Figure.Archer | Figure.IsBlack,
                (short)(PositionConstants.D1 + PositionConstants.D1), // distance 2 down
                FigureActionType.RangedAttack
            )
        });
    }
}
