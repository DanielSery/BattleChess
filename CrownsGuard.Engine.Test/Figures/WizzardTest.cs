using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class WizzardTest
{
    [Fact]
    public Task ExecuteWizzardMove_Verify()
    {
        const int src = 27; // d4 (3,3)
        const Figure whiteWizzard = Figure.Wizzard | Figure.IsWhite;
        const Figure blackWizzard = Figure.Wizzard | Figure.IsBlack;

        return Verify(new List<object>
        {
            // Test diagonal moves (should trigger cardinal direction destruction)
            TestUtils.RunExecuteActionScenario("White wizzard diagonal move up-right to (1,5) - destroys cardinal directions", b =>
                {
                    // Place figures in cardinal directions from target position (1,5)
                    // Target position is (1,5) which is index 1*8 + 5 = 13
                    // Cardinal directions from target: R1, L1, U1, D1
                    var targetIndex = 13; // (1,5)
                    var rightPos = targetIndex.GetWithOffset(PositionConstants.R1);
                    if (rightPos != -1) b[rightPos] = Figure.Peasant | Figure.IsBlack; // right from target
                    var leftPos = targetIndex.GetWithOffset(PositionConstants.L1);
                    if (leftPos != -1) b[leftPos] = Figure.Peasant | Figure.IsBlack; // left from target
                    var upPos = targetIndex.GetWithOffset(PositionConstants.U1);
                    if (upPos != -1) b[upPos] = Figure.Peasant | Figure.IsBlack; // up from target
                    var downPos = targetIndex.GetWithOffset(PositionConstants.D1);
                    if (downPos != -1) b[downPos] = Figure.Peasant | Figure.IsBlack; // down from target
                },
                src, whiteWizzard,
                PositionConstants.U2R2,
                FigureActionType.Move
            ),

            // Simple test to verify destruction works at all - use WizzardMove action type
            TestUtils.RunExecuteActionScenario("Simple destruction test - place figure adjacent to source", b =>
                {
                    // Place a figure that should be destroyed by diagonal move
                    // Source is at (3,3) = 27, moving U2R2 to (1,5) = 13
                    // For diagonal move, should destroy from source position in cardinal directions
                    // Let's place a figure right next to source that should be destroyed
                    var rightFromSource = src.GetWithOffset(PositionConstants.R1);
                    if (rightFromSource != -1) b[rightFromSource] = Figure.Peasant | Figure.IsBlack;
                },
                src, whiteWizzard,
                PositionConstants.U2R2,
                FigureActionType.WizzardMove
            ),

            TestUtils.RunExecuteActionScenario("White wizzard diagonal move up-left to (1,1) - destroys cardinal directions", _ =>
                {
                    // Place some figures to be destroyed in cardinal directions from source
                },
                src, whiteWizzard,
                PositionConstants.U2L2,
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario("White wizzard diagonal move down-right to (5,5) - destroys cardinal directions", _ =>
                {
                    // Place some figures to be destroyed in cardinal directions from source
                },
                src, whiteWizzard,
                PositionConstants.D2R2,
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario("White wizzard diagonal move down-left to (5,1) - destroys cardinal directions", _ =>
                {
                    // Place some figures to be destroyed in cardinal directions from source
                },
                src, whiteWizzard,
                PositionConstants.D2L2,
                FigureActionType.Move
            ),

            // Test non-diagonal moves (should trigger diagonal direction destruction)
            TestUtils.RunExecuteActionScenario("White wizzard straight up to (1,3) - destroys diagonal directions", b =>
                {
                    // Place figures in diagonal directions from target position (1,3)
                    // Target position is (1,3) which is index 1*8 + 3 = 11
                    // Diagonal directions from target: D1R1, U1L1, U1R1, D1L1
                    var targetIndex = 11; // (1,3)
                    var drPos = targetIndex.GetWithOffset(PositionConstants.D1R1);
                    if (drPos != -1) b[drPos] = Figure.Peasant | Figure.IsBlack; // down-right from target
                    var ulPos = targetIndex.GetWithOffset(PositionConstants.U1L1);
                    if (ulPos != -1) b[ulPos] = Figure.Peasant | Figure.IsBlack; // up-left from target
                    var urPos = targetIndex.GetWithOffset(PositionConstants.U1R1);
                    if (urPos != -1) b[urPos] = Figure.Peasant | Figure.IsBlack; // up-right from target
                    var dlPos = targetIndex.GetWithOffset(PositionConstants.D1L1);
                    if (dlPos != -1) b[dlPos] = Figure.Peasant | Figure.IsBlack; // down-left from target
                },
                src, whiteWizzard,
                PositionConstants.U2,
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario("White wizzard straight down to (5,3) - destroys diagonal directions", _ =>
                {
                    // Place some figures to be destroyed in diagonal directions from source
                },
                src, whiteWizzard,
                PositionConstants.D2,
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario("White wizzard straight left to (3,1) - destroys diagonal directions", _ =>
                {
                    // Place some figures to be destroyed in diagonal directions from source
                },
                src, whiteWizzard,
                PositionConstants.L2,
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario("White wizzard straight right to (3,5) - destroys diagonal directions", _ =>
                {
                    // Place some figures to be destroyed in diagonal directions from source
                },
                src, whiteWizzard,
                PositionConstants.R2,
                FigureActionType.Move
            ),

            // Test black wizzard moves
            TestUtils.RunExecuteActionScenario("Black wizzard diagonal move up-right to (1,5) - destroys cardinal directions", _ =>
                {
                    // Place some figures to be destroyed in cardinal directions from source
                },
                src, blackWizzard,
                PositionConstants.U2R2,
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario("Black wizzard diagonal move down-left to (5,1) - destroys cardinal directions", _ =>
                {
                    // Place some figures to be destroyed in cardinal directions from source
                },
                src, blackWizzard,
                PositionConstants.D2L2,
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario("Black wizzard straight up to (1,3) - destroys diagonal directions", _ =>
                {
                    // Place some figures to be destroyed in diagonal directions from source
                },
                src, blackWizzard,
                PositionConstants.U2,
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario("Black wizzard straight down to (5,3) - destroys diagonal directions", _ =>
                {
                    // Place some figures to be destroyed in diagonal directions from source
                },
                src, blackWizzard,
                PositionConstants.D2,
                FigureActionType.Move
            ),

            // Test blocked moves
            TestUtils.RunExecuteActionScenario("White wizzard move blocked by friendly unit", b =>
                {
                    // Block the target position with friendly unit
                    var target = src.GetWithOffset(PositionConstants.U2R2);
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsWhite; // friendly blocks move
                },
                src, whiteWizzard,
                PositionConstants.U2R2,
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario("White wizzard move blocked by enemy unit", b =>
                {
                    // Block the target position with enemy unit
                    var target = src.GetWithOffset(PositionConstants.U2L2);
                    if (target != -1) b[target] = Figure.Peasant | Figure.IsBlack; // enemy blocks move
                },
                src, whiteWizzard,
                PositionConstants.U2L2,
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario("White wizzard move blocked by wall", b =>
                {
                    // Block the target position with wall
                    var target = src.GetWithOffset(PositionConstants.D2R2);
                    if (target != -1) b[target] = Figure.Wall; // wall blocks move
                },
                src, whiteWizzard,
                PositionConstants.D2R2,
                FigureActionType.Move
            ),

            // Test edge cases
            TestUtils.RunExecuteActionScenario("Edge case - White wizzard move from corner A1", _ =>
                {
                    // Test move from corner position
                },
                0, // a1 (0,0)
                whiteWizzard,
                PositionConstants.U2R2, // Should be valid from corner
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenario("Edge case - White wizzard diagonal move from near edge", _ =>
                {
                    // Test diagonal move that would destroy tiles near board edge
                },
                9, // a2 (1,0) - near left edge
                whiteWizzard,
                PositionConstants.U2R2, // Diagonal move that might be affected by edge
                FigureActionType.Move
            )
        });
    }
}