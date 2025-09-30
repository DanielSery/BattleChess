using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class WhiplashTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure whiplash = Figure.Whiplash | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty around */ },
                src,
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Enemy in knight positions",
                b =>
                {
                    // Place enemies in all 8 knight positions from d4
                    var u1r2 = src.GetWithOffset(PositionConstants.U1R2);
                    var u2r1 = src.GetWithOffset(PositionConstants.U2R1);
                    var d1r2 = src.GetWithOffset(PositionConstants.D1R2);
                    var d2r1 = src.GetWithOffset(PositionConstants.D2R1);
                    var u1l2 = src.GetWithOffset(PositionConstants.U1L2);
                    var u2l1 = src.GetWithOffset(PositionConstants.U2L1);
                    var d1l2 = src.GetWithOffset(PositionConstants.D1L2);
                    var d2l1 = src.GetWithOffset(PositionConstants.D2L1);

                    if (u1r2 != -1) b[u1r2] = Figure.Peasant | Figure.IsBlack;
                    if (u2r1 != -1) b[u2r1] = Figure.Archer | Figure.IsBlack;
                    if (d1r2 != -1) b[d1r2] = Figure.Knight | Figure.IsBlack;
                    if (d2r1 != -1) b[d2r1] = Figure.Mage | Figure.IsBlack;
                    if (u1l2 != -1) b[u1l2] = Figure.Trader | Figure.IsBlack;
                    if (u2l1 != -1) b[u2l1] = Figure.LegionarySword | Figure.IsBlack;
                    if (d1l2 != -1) b[d1l2] = Figure.Peasant | Figure.IsBlack;
                    if (d2l1 != -1) b[d2l1] = Figure.Archer | Figure.IsBlack;
                },
                src,
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Allies in knight positions",
                b =>
                {
                    // Place allies in all 8 knight positions from d4
                    var u1r2 = src.GetWithOffset(PositionConstants.U1R2);
                    var u2r1 = src.GetWithOffset(PositionConstants.U2R1);
                    var d1r2 = src.GetWithOffset(PositionConstants.D1R2);
                    var d2r1 = src.GetWithOffset(PositionConstants.D2R1);
                    var u1l2 = src.GetWithOffset(PositionConstants.U1L2);
                    var u2l1 = src.GetWithOffset(PositionConstants.U2L1);
                    var d1l2 = src.GetWithOffset(PositionConstants.D1L2);
                    var d2l1 = src.GetWithOffset(PositionConstants.D2L1);

                    if (u1r2 != -1) b[u1r2] = Figure.Peasant | Figure.IsWhite;
                    if (u2r1 != -1) b[u2r1] = Figure.Archer | Figure.IsWhite;
                    if (d1r2 != -1) b[d1r2] = Figure.Knight | Figure.IsWhite;
                    if (d2r1 != -1) b[d2r1] = Figure.Mage | Figure.IsWhite;
                    if (u1l2 != -1) b[u1l2] = Figure.Trader | Figure.IsWhite;
                    if (u2l1 != -1) b[u2l1] = Figure.LegionarySword | Figure.IsWhite;
                    if (d1l2 != -1) b[d1l2] = Figure.Peasant | Figure.IsWhite;
                    if (d2l1 != -1) b[d2l1] = Figure.Archer | Figure.IsWhite;
                },
                src,
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Mixed blocking scenario",
                b =>
                {
                    // Place various figures to test different action types
                    var u1r2 = src.GetWithOffset(PositionConstants.U1R2);
                    var u2r1 = src.GetWithOffset(PositionConstants.U2R1);
                    var d1r2 = src.GetWithOffset(PositionConstants.D1R2);
                    var d2r1 = src.GetWithOffset(PositionConstants.D2R1);
                    var u1l2 = src.GetWithOffset(PositionConstants.U1L2);
                    var u2l1 = src.GetWithOffset(PositionConstants.U2L1);
                    var d1l2 = src.GetWithOffset(PositionConstants.D1L2);
                    var d2l1 = src.GetWithOffset(PositionConstants.D2L1);

                    // Empty squares for moves
                    if (u1r2 != -1) b[u1r2] = Figure.Empty;
                    if (u2r1 != -1) b[u2r1] = Figure.Empty;

                    // Enemy for attack
                    if (d1r2 != -1) b[d1r2] = Figure.Peasant | Figure.IsBlack;

                    // Ally for defend
                    if (d2r1 != -1) b[d2r1] = Figure.Archer | Figure.IsWhite;

                    // Wall for blocking
                    if (u1l2 != -1) b[u1l2] = Figure.Wall;

                    // Another enemy for attack
                    if (u2l1 != -1) b[u2l1] = Figure.Knight | Figure.IsBlack;

                    // Another ally for defend
                    if (d1l2 != -1) b[d1l2] = Figure.Mage | Figure.IsWhite;

                    // Neutral figure for possible attack
                    if (d2l1 != -1) b[d2l1] = Figure.Trader; // No color flag = neutral
                },
                src,
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Corner edge case A1",
                _ => { /* setup below places whiplash at edge via src override */ },
                0, // a1
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Corner edge case H1",
                _ => { /* setup below places whiplash at edge via src override */ },
                7, // h1
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Corner edge case A8",
                _ => { /* setup below places whiplash at edge via src override */ },
                56, // a8
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Corner edge case H8",
                _ => { /* setup below places whiplash at edge via src override */ },
                63, // h8
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Side edge case A4",
                _ => { /* setup below places whiplash at edge via src override */ },
                24, // a4
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Side edge case H4",
                _ => { /* setup below places whiplash at edge via src override */ },
                31, // h4
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Side edge case D1",
                _ => { /* setup below places whiplash at edge via src override */ },
                3, // d1
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Side edge case D8",
                _ => { /* setup below places whiplash at edge via src override */ },
                59, // d8
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Near-corner edge case B2",
                _ => { /* setup below places whiplash at edge via src override */ },
                9, // b2
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Near-corner edge case G2",
                _ => { /* setup below places whiplash at edge via src override */ },
                14, // g2
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Near-corner edge case B7",
                _ => { /* setup below places whiplash at edge via src override */ },
                49, // b7
                whiplash,
                Whiplash.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Near-corner edge case G7",
                _ => { /* setup below places whiplash at edge via src override */ },
                54, // g7
                whiplash,
                Whiplash.GetPossibleActions
            )
        });
    }

    [Fact]
    public Task ExecuteMeleeAction_Verify()
    {
        const int src = 27; // d4
        const Figure whiteWhiplash = Figure.Whiplash | Figure.IsWhite;
        const Figure blackWhiplash = Figure.Whiplash | Figure.IsBlack;

        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario(
                "White whiplash attacks enemy at (2,1) distance - up right",
                b =>
                {
                    var target = src.GetWithOffset(PositionConstants.U2R1);
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsBlack; // enemy to attack
                },
                src,
                whiteWhiplash,
                PositionConstants.U2R1,
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenario(
                "White whiplash attacks enemy at (2,1) distance - up left",
                b =>
                {
                    var target = src.GetWithOffset(PositionConstants.U2L1);
                    if (target != -1) b[target] = Figure.Archer | Figure.IsBlack; // enemy to attack
                },
                src,
                whiteWhiplash,
                PositionConstants.U2L1,
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenario(
                "White whiplash attacks enemy at (1,2) distance - up right",
                b =>
                {
                    var target = src.GetWithOffset(PositionConstants.U1R2);
                    if (target != -1) b[target] = Figure.Mage | Figure.IsBlack; // enemy to attack
                },
                src,
                whiteWhiplash,
                PositionConstants.U1R2,
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenario(
                "White whiplash attacks enemy at (1,2) distance - up left",
                b =>
                {
                    var target = src.GetWithOffset(PositionConstants.U1L2);
                    if (target != -1) b[target] = Figure.Trader | Figure.IsBlack; // enemy to attack
                },
                src,
                whiteWhiplash,
                PositionConstants.U1L2,
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenario(
                "Black whiplash attacks enemy at (2,1) distance - down right",
                b =>
                {
                    var target = src.GetWithOffset(PositionConstants.D2R1);
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsWhite; // enemy to attack
                },
                src,
                blackWhiplash,
                PositionConstants.D2R1,
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenario(
                "Black whiplash attacks enemy at (2,1) distance - down left",
                b =>
                {
                    var target = src.GetWithOffset(PositionConstants.D2L1);
                    if (target != -1) b[target] = Figure.Archer | Figure.IsWhite; // enemy to attack
                },
                src,
                blackWhiplash,
                PositionConstants.D2L1,
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenario(
                "Black whiplash attacks enemy at (1,2) distance - down right",
                b =>
                {
                    var target = src.GetWithOffset(PositionConstants.D1R2);
                    if (target != -1) b[target] = Figure.Mage | Figure.IsWhite; // enemy to attack
                },
                src,
                blackWhiplash,
                PositionConstants.D1R2,
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenario(
                "Black whiplash attacks enemy at (1,2) distance - down left",
                b =>
                {
                    var target = src.GetWithOffset(PositionConstants.D1L2);
                    if (target != -1) b[target] = Figure.Trader | Figure.IsWhite; // enemy to attack
                },
                src,
                blackWhiplash,
                PositionConstants.D1L2,
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenario(
                "White whiplash moves to empty square at (2,1) distance",
                b =>
                {
                    var target = src.GetWithOffset(PositionConstants.U2R1);
                    if (target != -1) b[target] = Figure.Empty; // empty square to move to
                },
                src,
                whiteWhiplash,
                PositionConstants.U2R1,
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario(
                "White whiplash moves to empty square at (1,2) distance",
                b =>
                {
                    var target = src.GetWithOffset(PositionConstants.U1R2);
                    if (target != -1) b[target] = Figure.Empty; // empty square to move to
                },
                src,
                whiteWhiplash,
                PositionConstants.U1R2,
                FigureActionType.Move
            )
        });
    }
}