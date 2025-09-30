using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class BuilderTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure builder = Figure.Builder | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty board */ },
                src,
                builder,
                Builder.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Walls adjacent",
                b =>
                {
                    // Place walls on Up and Right to trigger MeleeAttack, empties on Down/Left to allow BuildWall
                    var u = src.GetWithOffset(PositionConstants.U1);
                    var r = src.GetWithOffset(PositionConstants.R1);
                    var d = src.GetWithOffset(PositionConstants.D1);
                    var l = src.GetWithOffset(PositionConstants.L1);
                    if (u != -1) b[u] = Figure.Wall;
                    if (r != -1) b[r] = Figure.Wall;
                    if (d != -1) b[d] = Figure.Empty;
                    if (l != -1) b[l] = Figure.Empty;

                    // Make all diagonals walkable to verify diagonal one-step moves
                    foreach (var rel in PositionConstants.BishopDirections)
                    {
                        var idx = src.GetWithOffset(rel);
                        if (idx != -1) b[idx] = Figure.Empty;
                    }
                },
                src,
                builder,
                Builder.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Mixed neighbors",
                b =>
                {
                    // Up empty -> BuildWall
                    var u = src.GetWithOffset(PositionConstants.U1);
                    if (u != -1) b[u] = Figure.Empty;
                    // Right friendly (non-empty non-wall) -> no action on rook direction
                    var r = src.GetWithOffset(PositionConstants.R1);
                    if (r != -1) b[r] = Figure.LegionarySword | Figure.IsWhite;
                    // Down enemy (non-empty non-wall) -> no action on rook direction
                    var d = src.GetWithOffset(PositionConstants.D1);
                    if (d != -1) b[d] = Figure.Peasant | Figure.IsBlack;
                    // Left wall -> MeleeAttack
                    var l = src.GetWithOffset(PositionConstants.L1);
                    if (l != -1) b[l] = Figure.Wall;

                    // Diagonals: make two walkable and two blocked to verify move generation only on walkable
                    var ul = src.GetWithOffset(PositionConstants.U1L1);
                    var ur = src.GetWithOffset(PositionConstants.U1R1);
                    var dl = src.GetWithOffset(PositionConstants.D1L1);
                    var dr = src.GetWithOffset(PositionConstants.D1R1);
                    if (ul != -1) b[ul] = Figure.Empty; // move
                    if (ur != -1) b[ur] = Figure.Wall;  // block
                    if (dl != -1) b[dl] = Figure.Empty; // move
                    if (dr != -1) b[dr] = Figure.Peasant | Figure.IsBlack; // not walkable
                },
                src,
                builder,
                Builder.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                _ => { },
                0, // a1
                builder,
                Builder.GetPossibleActions)
        });
    }

    [Fact]
    public Task ExecuteAction_Verify()
    {
        const int src = 27; // d4
        const Figure whiteBuilder = Figure.Builder | Figure.IsWhite;
        const Figure blackBuilder = Figure.Builder | Figure.IsBlack;

        return Verify(new List<object>
        {
            // White Builder - BuildWall on empty adjacent squares
            TestUtils.RunExecuteActionScenario("White BuildWall up", b =>
                {
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Empty; // Empty square to build wall on
                },
                src,
                whiteBuilder,
                PositionConstants.U1,
                FigureActionType.BuildWall),

            TestUtils.RunExecuteActionScenario("White BuildWall right", b =>
                {
                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Empty; // Empty square to build wall on
                },
                src,
                whiteBuilder,
                PositionConstants.R1,
                FigureActionType.BuildWall),

            TestUtils.RunExecuteActionScenario("White BuildWall down", b =>
                {
                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Empty; // Empty square to build wall on
                },
                src,
                whiteBuilder,
                PositionConstants.D1,
                FigureActionType.BuildWall),

            TestUtils.RunExecuteActionScenario("White BuildWall left", b =>
                {
                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Empty; // Empty square to build wall on
                },
                src,
                whiteBuilder,
                PositionConstants.L1,
                FigureActionType.BuildWall),

            // Black Builder - BuildWall on empty adjacent squares
            TestUtils.RunExecuteActionScenario("Black BuildWall up", b =>
                {
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Empty; // Empty square to build wall on
                },
                src,
                blackBuilder,
                PositionConstants.U1,
                FigureActionType.BuildWall),

            TestUtils.RunExecuteActionScenario("Black BuildWall right", b =>
                {
                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Empty; // Empty square to build wall on
                },
                src,
                blackBuilder,
                PositionConstants.R1,
                FigureActionType.BuildWall),

            TestUtils.RunExecuteActionScenario("Black BuildWall down", b =>
                {
                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Empty; // Empty square to build wall on
                },
                src,
                blackBuilder,
                PositionConstants.D1,
                FigureActionType.BuildWall),

            TestUtils.RunExecuteActionScenario("Black BuildWall left", b =>
                {
                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Empty; // Empty square to build wall on
                },
                src,
                blackBuilder,
                PositionConstants.L1,
                FigureActionType.BuildWall),

            // White Builder - MeleeAttack on adjacent walls
            TestUtils.RunExecuteActionScenario("White MeleeAttack wall up", b =>
                {
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Wall; // Wall to attack
                },
                src,
                whiteBuilder,
                PositionConstants.U1,
                FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("White MeleeAttack wall right", b =>
                {
                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Wall; // Wall to attack
                },
                src,
                whiteBuilder,
                PositionConstants.R1,
                FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("White MeleeAttack wall down", b =>
                {
                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Wall; // Wall to attack
                },
                src,
                whiteBuilder,
                PositionConstants.D1,
                FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("White MeleeAttack wall left", b =>
                {
                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Wall; // Wall to attack
                },
                src,
                whiteBuilder,
                PositionConstants.L1,
                FigureActionType.MeleeAttack),

            // Black Builder - MeleeAttack on adjacent walls
            TestUtils.RunExecuteActionScenario("Black MeleeAttack wall up", b =>
                {
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Wall; // Wall to attack
                },
                src,
                blackBuilder,
                PositionConstants.U1,
                FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("Black MeleeAttack wall right", b =>
                {
                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Wall; // Wall to attack
                },
                src,
                blackBuilder,
                PositionConstants.R1,
                FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("Black MeleeAttack wall down", b =>
                {
                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Wall; // Wall to attack
                },
                src,
                blackBuilder,
                PositionConstants.D1,
                FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("Black MeleeAttack wall left", b =>
                {
                    var left = src.GetWithOffset(PositionConstants.L1);
                    if (left != -1) b[left] = Figure.Wall; // Wall to attack
                },
                src,
                blackBuilder,
                PositionConstants.L1,
                FigureActionType.MeleeAttack),

            // Edge cases - BuildWall blocked scenarios
            TestUtils.RunExecuteActionScenario("White BuildWall blocked by friendly unit", b =>
                {
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Peasant | Figure.IsWhite; // Friendly unit blocks building
                },
                src,
                whiteBuilder,
                PositionConstants.U1,
                FigureActionType.BuildWall),

            TestUtils.RunExecuteActionScenario("White BuildWall blocked by enemy unit", b =>
                {
                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Archer | Figure.IsBlack; // Enemy unit blocks building
                },
                src,
                whiteBuilder,
                PositionConstants.R1,
                FigureActionType.BuildWall),

            TestUtils.RunExecuteActionScenario("White BuildWall blocked by existing wall", b =>
                {
                    var down = src.GetWithOffset(PositionConstants.D1);
                    if (down != -1) b[down] = Figure.Wall; // Existing wall blocks building
                },
                src,
                whiteBuilder,
                PositionConstants.D1,
                FigureActionType.BuildWall),

            // Edge cases - MeleeAttack blocked scenarios
            TestUtils.RunExecuteActionScenario("White MeleeAttack blocked by friendly unit", b =>
                {
                    var up = src.GetWithOffset(PositionConstants.U1);
                    if (up != -1) b[up] = Figure.Peasant | Figure.IsWhite; // Friendly unit instead of wall
                },
                src,
                whiteBuilder,
                PositionConstants.U1,
                FigureActionType.MeleeAttack),

            TestUtils.RunExecuteActionScenario("White MeleeAttack on empty square", b =>
                {
                    var right = src.GetWithOffset(PositionConstants.R1);
                    if (right != -1) b[right] = Figure.Empty; // Empty square instead of wall
                },
                src,
                whiteBuilder,
                PositionConstants.R1,
                FigureActionType.MeleeAttack),

            // Edge case - Corner position A1
            TestUtils.RunExecuteActionScenario("White BuildWall from corner A1", _ => { },
                0, // a1
                whiteBuilder,
                PositionConstants.R1, // Right from A1
                FigureActionType.BuildWall),

            TestUtils.RunExecuteActionScenario("White MeleeAttack from corner A1", b =>
                {
                    var right = 1; // b1 (right from a1)
                    b[right] = Figure.Wall; // Wall to attack
                },
                0, // a1
                whiteBuilder,
                PositionConstants.R1, // Right from A1
                FigureActionType.MeleeAttack)
        });
    }
}
