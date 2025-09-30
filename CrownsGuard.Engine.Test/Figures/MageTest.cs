using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class MageTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4 (3,3)
        const Figure mage = Figure.Mage | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty board around */ },
                src,
                mage,
                Mage.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Blocked targets", b =>
                {
                    // Block some of the 8 target tiles at distance 2
                    // Targets: (1,1), (1,3), (1,5), (3,1), (3,5), (5,1), (5,3), (5,5)
                    var t11 = 1 * 8 + 1; // (1,1)
                    var t15 = 1 * 8 + 5; // (1,5)
                    var t31 = 3 * 8 + 1; // (3,1)
                    var t55 = 5 * 8 + 5; // (5,5)

                    b[t11] = Figure.Peasant | Figure.IsWhite; // friendly blocks
                    b[t15] = Figure.Peasant | Figure.IsBlack; // enemy blocks (not walkable)
                    b[t31] = Figure.Wall; // wall blocks
                    b[t55] = Figure.Knight | Figure.IsBlack; // enemy blocks
                },
                src,
                mage,
                Mage.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                _ => { /* at corner, only in-bounds moves */ },
                0, // a1 (0,0)
                mage,
                Mage.GetPossibleActions)
        });
    }

    [Fact]
    public Task ExecuteMageMove_Verify()
    {
        const int src = 27; // d4 (3,3)
        const Figure whiteMage = Figure.Mage | Figure.IsWhite;
        const Figure blackMage = Figure.Mage | Figure.IsBlack;

        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario("White mage teleports up-right diagonally to (1,5)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, whiteMage,
                PositionConstants.U2R2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("White mage teleports up-left diagonally to (1,1)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, whiteMage,
                PositionConstants.U2L2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("White mage teleports down-right diagonally to (5,5)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, whiteMage,
                PositionConstants.D2R2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("White mage teleports down-left diagonally to (5,1)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, whiteMage,
                PositionConstants.D2L2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("White mage teleports straight up to (1,3)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, whiteMage,
                PositionConstants.U2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("White mage teleports straight down to (5,3)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, whiteMage,
                PositionConstants.D2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("White mage teleports straight left to (3,1)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, whiteMage,
                PositionConstants.L2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("White mage teleports straight right to (3,5)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, whiteMage,
                PositionConstants.R2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("Black mage teleports up-right diagonally to (1,5)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, blackMage,
                PositionConstants.U2R2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("Black mage teleports down-left diagonally to (5,1)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, blackMage,
                PositionConstants.D2L2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("Black mage teleports straight up to (1,3)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, blackMage,
                PositionConstants.U2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("Black mage teleports straight down to (5,3)", _ =>
                {
                    // Place empty target for teleportation
                },
                src, blackMage,
                PositionConstants.D2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("White mage teleport blocked by friendly unit", b =>
                {
                    // Block the target position with friendly unit
                    var target = src.GetWithOffset(PositionConstants.U2R2);
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsWhite; // friendly blocks teleport
                },
                src, whiteMage,
                PositionConstants.U2R2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("White mage teleport blocked by enemy unit", b =>
                {
                    // Block the target position with enemy unit
                    var target = src.GetWithOffset(PositionConstants.U2L2);
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsBlack; // enemy blocks teleport
                },
                src, whiteMage,
                PositionConstants.U2L2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("White mage teleport blocked by wall", b =>
                {
                    // Block the target position with wall
                    var target = src.GetWithOffset(PositionConstants.D2R2);
                    if (target != -1) b[target] = Figure.Wall; // wall blocks teleport
                },
                src, whiteMage,
                PositionConstants.D2R2,
                FigureActionType.MageMove
            ),

            TestUtils.RunExecuteActionScenario("Edge case - White mage teleport from corner A1", _ =>
                {
                    // Test teleport from corner position
                },
                0, // a1 (0,0)
                whiteMage,
                PositionConstants.U2R2, // Should be valid from corner
                FigureActionType.MageMove
            )
        });
    }
}
