using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class PeasantTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure whitePeasant = Figure.Peasant | Figure.IsWhite;
        const Figure blackPeasant = Figure.Peasant | Figure.IsBlack;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "White - All empty", _ => { /* empty around */ },
                src,
                whitePeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - All empty", _ => { /* empty around */ },
                src,
                blackPeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "White - Movement and attacks", b =>
                {
                    // White peasant moves up (backward) and attacks up/left/right
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Empty; // Can move here

                    var upLeft = src.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));
                    if (upLeft != -1) b[upLeft] = Figure.Peasant | Figure.IsBlack; // Can attack enemy

                    var upRight = src.GetWithOffset((short)(1 + -1 * PositionConstants.YOffset));
                    if (upRight != -1) b[upRight] = Figure.Archer | Figure.IsBlack; // Can attack enemy

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Knight | Figure.IsBlack; // Can attack enemy

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Mage | Figure.IsBlack; // Can attack enemy

                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Builder | Figure.IsWhite; // Friendly - cannot attack
                },
                src,
                whitePeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Black - Movement and attacks", b =>
                {
                    // Black peasant moves down (forward) and attacks down/left/right
                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Empty; // Can move here

                    var downLeft = src.GetWithOffset((short)(-1 + 1 * PositionConstants.YOffset));
                    if (downLeft != -1) b[downLeft] = Figure.Peasant | Figure.IsWhite; // Can attack enemy

                    var downRight = src.GetWithOffset((short)(1 + 1 * PositionConstants.YOffset));
                    if (downRight != -1) b[downRight] = Figure.Archer | Figure.IsWhite; // Can attack enemy

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Knight | Figure.IsWhite; // Can attack enemy

                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Mage | Figure.IsWhite; // Can attack enemy

                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Builder | Figure.IsBlack; // Friendly - cannot attack
                },
                src,
                blackPeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "White - Blocked movement", b =>
                {
                    // Block white peasant's movement and some attacks
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Wall; // Cannot move through wall

                    var upLeft = src.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));
                    if (upLeft != -1) b[upLeft] = Figure.Archer | Figure.IsWhite; // Friendly - cannot attack

                    var upRight = src.GetWithOffset((short)(1 + -1 * PositionConstants.YOffset));
                    if (upRight != -1) b[upRight] = Figure.Peasant | Figure.IsBlack; // Can attack enemy

                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Knight | Figure.IsBlack; // Can attack enemy
                },
                src,
                whitePeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("White - Edge case H1", _ => { /* setup below places peasant at edge via src override */ },
                7, // h1
                whitePeasant,
                Peasant.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Black - Edge case A8", _ => { /* setup below places peasant at edge via src override */ },
                56, // a8
                blackPeasant,
                Peasant.GetPossibleActions
            )
        });
    }

    [Fact]
    public Task ExecuteAction_Verify()
    {
        const int src = 27; // d4
        const Figure whitePeasant = Figure.Peasant | Figure.IsWhite;
        const Figure blackPeasant = Figure.Peasant | Figure.IsBlack;

        return Verify(new List<object>
        {
            // White peasant movement scenarios
            TestUtils.RunExecuteActionScenario("White peasant moves up to empty square", _ => { /* empty setup */ },
                src, whitePeasant, PositionConstants.U1, FigureActionType.Move),

            TestUtils.RunExecuteActionScenario("White peasant moves left to empty square", _ => { /* empty setup */ },
                src, whitePeasant, PositionConstants.L1, FigureActionType.Move),

            TestUtils.RunExecuteActionScenario("White peasant moves right to empty square", _ => { /* empty setup */ },
                src, whitePeasant, PositionConstants.R1, FigureActionType.Move),

            // Black peasant movement scenarios
            TestUtils.RunExecuteActionScenario("Black peasant moves down to empty square", _ => { /* empty setup */ },
                src, blackPeasant, PositionConstants.D1, FigureActionType.Move),

            TestUtils.RunExecuteActionScenario("Black peasant moves left to empty square", _ => { /* empty setup */ },
                src, blackPeasant, PositionConstants.L1, FigureActionType.Move),

            TestUtils.RunExecuteActionScenario("Black peasant moves right to empty square", _ => { /* empty setup */ },
                src, blackPeasant, PositionConstants.R1, FigureActionType.Move),

            // White peasant melee attack scenarios
            TestUtils.RunExecuteActionScenario("White peasant attacks enemy up", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.U1);
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsBlack; // enemy to attack
                },
                src, whitePeasant, PositionConstants.U1, FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("White peasant attacks enemy up-left", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.U1L1);
                    if (target != -1) b[target] = Figure.Archer | Figure.IsBlack; // enemy to attack
                },
                src, whitePeasant, PositionConstants.U1L1, FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("White peasant attacks enemy up-right", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.U1R1);
                    if (target != -1) b[target] = Figure.Knight | Figure.IsBlack; // enemy to attack
                },
                src, whitePeasant, PositionConstants.U1R1, FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("White peasant attacks enemy left", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.L1);
                    if (target != -1) b[target] = Figure.Mage | Figure.IsBlack; // enemy to attack
                },
                src, whitePeasant, PositionConstants.L1, FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("White peasant attacks enemy right", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.R1);
                    if (target != -1) b[target] = Figure.Trader | Figure.IsBlack; // enemy to attack
                },
                src, whitePeasant, PositionConstants.R1, FigureActionType.MeleeAttack),

            // Black peasant melee attack scenarios
            TestUtils.RunExecuteActionScenario("Black peasant attacks enemy down", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.D1);
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsWhite; // enemy to attack
                },
                src, blackPeasant, PositionConstants.D1, FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("Black peasant attacks enemy down-left", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.D1L1);
                    if (target != -1) b[target] = Figure.Archer | Figure.IsWhite; // enemy to attack
                },
                src, blackPeasant, PositionConstants.D1L1, FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("Black peasant attacks enemy down-right", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.D1R1);
                    if (target != -1) b[target] = Figure.Knight | Figure.IsWhite; // enemy to attack
                },
                src, blackPeasant, PositionConstants.D1R1, FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("Black peasant attacks enemy left", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.L1);
                    if (target != -1) b[target] = Figure.Mage | Figure.IsWhite; // enemy to attack
                },
                src, blackPeasant, PositionConstants.L1, FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("Black peasant attacks enemy right", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.R1);
                    if (target != -1) b[target] = Figure.Trader | Figure.IsWhite; // enemy to attack
                },
                src, blackPeasant, PositionConstants.R1, FigureActionType.MeleeAttack),

            // Edge cases - blocked actions
            TestUtils.RunExecuteActionScenario("White peasant blocked by wall", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.U1);
                    if (target != -1) b[target] = Figure.Wall; // cannot move through wall
                },
                src, whitePeasant, PositionConstants.U1, FigureActionType.Move),

            TestUtils.RunExecuteActionScenario("White peasant blocked by friendly unit", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.U1);
                    if (target != -1) b[target] = Figure.Builder | Figure.IsWhite; // cannot attack friendly
                },
                src, whitePeasant, PositionConstants.U1, FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("Black peasant blocked by wall", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.D1);
                    if (target != -1) b[target] = Figure.Wall; // cannot move through wall
                },
                src, blackPeasant, PositionConstants.D1, FigureActionType.Move),

            TestUtils.RunExecuteActionScenario("Black peasant blocked by friendly unit", b =>
                {
                    var target = src.GetWithOffset(PositionConstants.D1);
                    if (target != -1) b[target] = Figure.Builder | Figure.IsBlack; // cannot attack friendly
                },
                src, blackPeasant, PositionConstants.D1, FigureActionType.MeleeAttack),

            // Edge case - movement at board edge
            TestUtils.RunExecuteActionScenario("White peasant at edge - cannot move up", _ => { /* empty setup */ },
                7, // h1 - cannot move up from here
                whitePeasant, PositionConstants.U1, FigureActionType.Move),

            TestUtils.RunExecuteActionScenario("Black peasant at edge - cannot move down", _ => { /* empty setup */ },
                56, // a8 - cannot move down from here
                blackPeasant, PositionConstants.D1, FigureActionType.Move)
        });
    }
}