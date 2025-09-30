using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class FireTest
{
    [Fact]
    public Task OnMovedToFire_FiguresMoveToFire_CenterPositions()
    {
        const int firePosition = 27; // d4 - center of board
        const Figure fire = Figure.Fire;

        return Verify(new List<object>
        {
            // Non-Dragon Figures Die When Moving to Fire
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Empty board - peasant moves to fire",
                b => b[firePosition] = fire,
                firePosition.GetWithOffset(PositionConstants.U1), // Place peasant above fire
                Figure.Peasant | Figure.IsWhite,
                PositionConstants.D1, // Move down to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Mixed figures - archer moves to fire",
                b =>
                {
                    // Place fire at center
                    b[firePosition] = fire;

                    // Place various figures around fire
                    var u1 = firePosition.GetWithOffset(PositionConstants.U1);
                    var r1 = firePosition.GetWithOffset(PositionConstants.R1);
                    var d1 = firePosition.GetWithOffset(PositionConstants.D1);
                    var l1 = firePosition.GetWithOffset(PositionConstants.L1);

                    if (u1 != -1) b[u1] = Figure.Archer | Figure.IsWhite;
                    if (r1 != -1) b[r1] = Figure.Knight | Figure.IsBlack;
                    if (d1 != -1) b[d1] = Figure.Builder | Figure.IsWhite;
                    if (l1 != -1) b[l1] = Figure.Mage | Figure.IsBlack;
                },
                firePosition.GetWithOffset(PositionConstants.U1), // Archer moves from above to fire
                Figure.Archer | Figure.IsWhite,
                PositionConstants.D1, // Move down to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Empty board - knight moves to fire",
                b => b[firePosition] = fire,
                firePosition.GetWithOffset(PositionConstants.U2), // Place knight two spaces above fire
                Figure.Knight | Figure.IsWhite,
                PositionConstants.D2, // Move down to fire (two spaces)
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Knight moves to fire with figures in path",
                b =>
                {
                    // Place fire at center
                    b[firePosition] = fire;

                    // Place a figure between knight and fire
                    var u1 = firePosition.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Peasant | Figure.IsBlack; // Will be jumped over

                    // Place knight two spaces away
                    var u2 = firePosition.GetWithOffset(PositionConstants.U2);
                    if (u2 != -1) b[u2] = Figure.Knight | Figure.IsWhite;
                },
                firePosition.GetWithOffset(PositionConstants.U2), // Knight moves from two spaces above to fire
                Figure.Knight | Figure.IsWhite,
                PositionConstants.D2, // Move down to fire
                FigureActionType.Move
            ),

            // Dragon Immunity Tests
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Empty board - white dragon moves to fire (immune)",
                b => b[firePosition] = fire,
                firePosition.GetWithOffset(PositionConstants.U1), // Place dragon above fire
                Figure.Dragon | Figure.IsWhite,
                PositionConstants.D1, // Move down to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Empty board - black dragon moves to fire (immune)",
                b => b[firePosition] = fire,
                firePosition.GetWithOffset(PositionConstants.U1), // Place dragon above fire
                Figure.Dragon | Figure.IsBlack,
                PositionConstants.D1, // Move down to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Dragon moves to fire with other figures around",
                b =>
                {
                    // Place fire at center
                    b[firePosition] = fire;

                    // Place dragon above fire
                    var u1 = firePosition.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Dragon | Figure.IsWhite;

                    // Place other figures around
                    var r1 = firePosition.GetWithOffset(PositionConstants.R1);
                    var l1 = firePosition.GetWithOffset(PositionConstants.L1);
                    if (r1 != -1) b[r1] = Figure.Peasant | Figure.IsBlack;
                    if (l1 != -1) b[l1] = Figure.Archer | Figure.IsBlack;
                },
                firePosition.GetWithOffset(PositionConstants.U1), // Dragon moves from above to fire
                Figure.Dragon | Figure.IsWhite,
                PositionConstants.D1, // Move down to fire
                FigureActionType.Move
            )
        });
    }

    [Fact]
    public Task OnMovedToFire_FiguresMoveToFire_CornerPositions()
    {
        const Figure fire = Figure.Fire;

        return Verify(new List<object>
        {
            // Corner Position Tests - Non-Dragon Figures Die
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A1 corner - peasant moves to fire from right",
                b => b[0] = fire,
                1, // b1 - place peasant right of A1 fire
                Figure.Peasant | Figure.IsWhite,
                PositionConstants.L1, // Move left to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "H1 corner - archer moves to fire from left",
                b => b[7] = fire,
                6, // g1 - place archer left of H1 fire
                Figure.Archer | Figure.IsBlack,
                PositionConstants.R1, // Move right to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A8 corner - knight moves to fire from right",
                b => b[56] = fire,
                57, // b8 - place knight right of A8 fire
                Figure.Knight | Figure.IsWhite,
                PositionConstants.L1, // Move left to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "H8 corner - builder moves to fire from left",
                b => b[63] = fire,
                62, // g8 - place builder left of H8 fire
                Figure.Builder | Figure.IsBlack,
                PositionConstants.R1, // Move right to fire
                FigureActionType.Move
            ),

            // Corner Position Tests - Dragon Immunity
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A1 corner - white dragon moves to fire from right (immune)",
                b => b[0] = fire,
                1, // b1 - place dragon right of A1 fire
                Figure.Dragon | Figure.IsWhite,
                PositionConstants.L1, // Move left to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "H8 corner - black dragon moves to fire from left (immune)",
                b => b[63] = fire,
                62, // g8 - place dragon left of H8 fire
                Figure.Dragon | Figure.IsBlack,
                PositionConstants.R1, // Move right to fire
                FigureActionType.Move
            )
        });
    }

    [Fact]
    public Task OnMovedToFire_FiguresMoveToFire_SideEdgePositions()
    {
        const Figure fire = Figure.Fire;

        return Verify(new List<object>
        {
            // Side Edge Position Tests - Non-Dragon Figures Die
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "B1 side edge - peasant moves to fire from right",
                b => b[1] = fire,
                2, // c1 - place peasant right of B1 fire
                Figure.Peasant | Figure.IsWhite,
                PositionConstants.L1, // Move left to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "G1 side edge - archer moves to fire from right",
                b => b[6] = fire,
                5, // f1 - place archer left of G1 fire
                Figure.Archer | Figure.IsBlack,
                PositionConstants.R1, // Move right to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A4 side edge - knight moves to fire from below",
                b => b[24] = fire,
                32, // a5 - place knight below A4 fire
                Figure.Knight | Figure.IsWhite,
                PositionConstants.U1, // Move up to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "H4 side edge - builder moves to fire from below",
                b => b[31] = fire,
                39, // h5 - place builder below H4 fire
                Figure.Builder | Figure.IsBlack,
                PositionConstants.U1, // Move up to fire
                FigureActionType.Move
            ),

            // Side Edge Position Tests - Dragon Immunity
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "B1 side edge - white dragon moves to fire from right (immune)",
                b => b[1] = fire,
                2, // c1 - place dragon right of B1 fire
                Figure.Dragon | Figure.IsWhite,
                PositionConstants.L1, // Move left to fire
                FigureActionType.Move
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A4 side edge - black dragon moves to fire from below (immune)",
                b => b[24] = fire,
                32, // a5 - place dragon below A4 fire
                Figure.Dragon | Figure.IsBlack,
                PositionConstants.U1, // Move up to fire
                FigureActionType.Move
            )
        });
    }
}