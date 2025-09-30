using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class LegionarySwordTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty around */ },
                27, // d4
                Figure.LegionarySword | Figure.IsWhite,
                LegionarySword.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Start row double step white",
                _ => { /* ensure path ahead is empty by default */ },
                (6 * 8) + 3, // d7
                Figure.LegionarySword | Figure.IsWhite,
                LegionarySword.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Enemy left ally right white",
                b =>
                {
                    var src = 27; // d4
                    var ul = src.GetWithOffset(PositionConstants.U1L1);
                    var ur = src.GetWithOffset(PositionConstants.U1R1);
                    if (ul != -1) b[ul] = Figure.Peasant | Figure.IsBlack; // enemy -> MeleeAttack
                    if (ur != -1) b[ur] = Figure.Peasant | Figure.IsWhite; // ally -> MeleeDefend
                },
                27, // d4
                Figure.LegionarySword | Figure.IsWhite,
                LegionarySword.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Forward blocked no move white",
                b =>
                {
                    var src = 27; // d4
                    var forward = src.GetWithOffset(PositionConstants.D1); // white forward is D (-Y)
                    if (forward != -1) b[forward] = Figure.Wall; // non-walkable blocks move
                },
                27, // d4
                Figure.LegionarySword | Figure.IsWhite,
                LegionarySword.GetPossibleActions),

            new
            {
                White = TestUtils.RunGetActionsScenario(
                    "Promotion white forward square empty diagonals empty",
                    _ => { /* forward square empty, diagonals empty */ },
                    1 * 8 + 3, // d2 -> d1 (row 0)
                    Figure.LegionarySword | Figure.IsWhite,
                    LegionarySword.GetPossibleActions),
                Black = TestUtils.RunGetActionsScenario(
                    "Promotion black forward square empty diagonals empty",
                    _ => { /* forward square empty, diagonals empty */ },
                    6 * 8 + 4, // e7 -> e8 (row 7)
                    Figure.LegionarySword | Figure.IsBlack,
                    LegionarySword.GetPossibleActions)
            }
        });
    }

    [Fact]
    public Task ExecuteAction_Verify()
    {
        return Verify(new List<object>
        {
            // Regular Move execution - White
            TestUtils.RunExecuteActionScenario(
                "White regular move to empty adjacent square",
                _ => { /* target square is empty by default */ },
                27, // d4
                Figure.LegionarySword | Figure.IsWhite,
                PositionConstants.D1,
                FigureActionType.Move),

            // Regular Move execution - Black
            TestUtils.RunExecuteActionScenario(
                "Black regular move to empty adjacent square",
                _ => { /* target square is empty by default */ },
                27, // d4
                Figure.LegionarySword | Figure.IsBlack,
                PositionConstants.U1,
                FigureActionType.Move),

            // Double step Move execution from starting row - White
            TestUtils.RunExecuteActionScenario(
                "White double step move from starting row",
                _ => { /* both target squares are empty by default */ },
                (6 * 8) + 3, // d7 - white starting row
                Figure.LegionarySword | Figure.IsWhite,
                PositionConstants.D2,
                FigureActionType.Move),

            // Double step Move execution from starting row - Black
            TestUtils.RunExecuteActionScenario(
                "Black double step move from starting row",
                _ => { /* both target squares are empty by default */ },
                (1 * 8) + 3, // d2 - black starting row
                Figure.LegionarySword | Figure.IsBlack,
                PositionConstants.U2,
                FigureActionType.Move),

            // MeleeAttack execution - White attacking enemy on diagonal
            TestUtils.RunExecuteActionScenario(
                "White melee attack on adjacent enemy diagonal - up right",
                b =>
                {
                    var src = 27; // d4
                    var target = src.GetWithOffset(PositionConstants.D1R1); // white attack right
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsBlack; // enemy to attack
                },
                27, // d4
                Figure.LegionarySword | Figure.IsWhite,
                PositionConstants.D1R1,
                FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario(
                "White melee attack on adjacent enemy diagonal - up left",
                b =>
                {
                    var src = 27; // d4
                    var target = src.GetWithOffset(PositionConstants.D1L1); // white attack left
                    if (target != -1) b[target] = Figure.Archer | Figure.IsBlack; // enemy to attack
                },
                27, // d4
                Figure.LegionarySword | Figure.IsWhite,
                PositionConstants.D1L1, // white attack left
                FigureActionType.MeleeAttack),

            // MeleeAttack execution - Black attacking enemy on diagonal
            TestUtils.RunExecuteActionScenario(
                "Black melee attack on adjacent enemy diagonal - down right",
                b =>
                {
                    var src = 27; // d4
                    var target = src.GetWithOffset(PositionConstants.U1R1); // black attack right
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsWhite; // enemy to attack
                },
                27, // d4
                Figure.LegionarySword | Figure.IsBlack,
                PositionConstants.U1R1,
                FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario(
                "Black melee attack on adjacent enemy diagonal - down left",
                b =>
                {
                    var src = 27; // d4
                    var target = src.GetWithOffset(PositionConstants.U1L1); // black attack left
                    if (target != -1) b[target] = Figure.Archer | Figure.IsWhite; // enemy to attack
                },
                27, // d4
                Figure.LegionarySword | Figure.IsBlack,
                PositionConstants.U1L1,
                FigureActionType.MeleeAttack),

            // Edge case - Move blocked by wall
            TestUtils.RunExecuteActionScenario(
                "White move blocked by wall",
                b =>
                {
                    var src = 27; // d4
                    var target = src.GetWithOffset(PositionConstants.D1); // U1
                    if (target != -1) b[target] = Figure.Wall; // block the move
                },
                27, // d4
                Figure.LegionarySword | Figure.IsWhite,
                PositionConstants.D1,
                FigureActionType.Move),

            // Edge case - MeleeAttack blocked by friendly piece
            TestUtils.RunExecuteActionScenario(
                "White melee attack blocked by friendly piece",
                b =>
                {
                    var src = 27; // d4
                    var target = src.GetWithOffset(PositionConstants.D1R1); // white attack right
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsWhite; // friendly piece blocks attack
                },
                27, // d4
                Figure.LegionarySword | Figure.IsWhite,
                PositionConstants.D1R1,
                FigureActionType.MeleeAttack)
        });
    }
}
            