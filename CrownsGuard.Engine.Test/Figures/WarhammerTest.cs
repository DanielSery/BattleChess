using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class WarhammerTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure warhammer = Figure.Warhammer | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty", _ => { /* empty around */ },
                src,
                warhammer,
                Warhammer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Diagonal movement", b =>
                {
                    // Clear diagonal paths for movement
                    var ul = src.GetWithOffset(PositionConstants.U1L1);
                    if (ul != -1) b[ul] = Figure.Empty;

                    var ur = src.GetWithOffset(PositionConstants.U1R1);
                    if (ur != -1) b[ur] = Figure.Empty;

                    var dl = src.GetWithOffset(PositionConstants.D1L1);
                    if (dl != -1) b[dl] = Figure.Empty;

                    var dr = src.GetWithOffset(PositionConstants.D1R1);
                    if (dr != -1) b[dr] = Figure.Empty;
                },
                src,
                warhammer,
                Warhammer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario(
                "Blocked diagonals", b =>
                {
                    // Block some diagonal movement
                    var ul = src.GetWithOffset(PositionConstants.U1L1);
                    if (ul != -1) b[ul] = Figure.Wall; // Wall blocks

                    var ur = src.GetWithOffset(PositionConstants.U1R1);
                    if (ur != -1) b[ur] = Figure.Peasant | Figure.IsBlack; // Enemy blocks

                    var dl = src.GetWithOffset(PositionConstants.D1L1);
                    if (dl != -1) b[dl] = Figure.Archer | Figure.IsWhite; // Friendly blocks

                    var dr = src.GetWithOffset(PositionConstants.D1R1);
                    if (dr != -1) b[dr] = Figure.Empty; // Clear for movement
                },
                src,
                warhammer,
                Warhammer.GetPossibleActions
            ),

            TestUtils.RunGetActionsScenario("Edge case A1", _ => { /* setup below places warhammer at edge via src override */ },
                0, // a1
                warhammer,
                Warhammer.GetPossibleActions
            )
        });
    }

    [Fact]
    public Task ExecuteMove_DestructionPatterns_Verify()
    {
        const int src = 27; // d4
        const Figure warhammer = Figure.Warhammer | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario(
                "Right movement destroys right column", b =>
                {
                    // Setup: move right, place figures in the destruction column
                    var target = src.GetWithOffset(PositionConstants.R1);
                    var destroy1 = target.GetWithOffset((short)(1 + -1 * PositionConstants.YOffset));
                    var destroy2 = target.GetWithOffset((short)(1 + 0 * PositionConstants.YOffset));
                    var destroy3 = target.GetWithOffset((short)(1 + 1 * PositionConstants.YOffset));

                    if (destroy1 != -1) b[destroy1] = Figure.Peasant | Figure.IsBlack;
                    if (destroy2 != -1) b[destroy2] = Figure.Archer | Figure.IsBlack;
                    if (destroy3 != -1) b[destroy3] = Figure.Knight | Figure.IsBlack;
                },
                src,
                warhammer,
                PositionConstants.R1,
                FigureActionType.WarhammerMove
            ),

            TestUtils.RunExecuteActionScenario(
                "Left movement destroys left column", b =>
                {
                    // Setup: move left, place figures in the destruction column
                    var target = src.GetWithOffset(PositionConstants.L1);
                    var destroy1 = target.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));
                    var destroy2 = target.GetWithOffset((short)(-1 + 0 * PositionConstants.YOffset));
                    var destroy3 = target.GetWithOffset((short)(-1 + 1 * PositionConstants.YOffset));

                    if (destroy1 != -1) b[destroy1] = Figure.Peasant | Figure.IsBlack;
                    if (destroy2 != -1) b[destroy2] = Figure.Archer | Figure.IsBlack;
                    if (destroy3 != -1) b[destroy3] = Figure.Knight | Figure.IsBlack;
                },
                src,
                warhammer,
                PositionConstants.L1,
                FigureActionType.WarhammerMove
            ),

            TestUtils.RunExecuteActionScenario(
                "Up movement destroys upper row", b =>
                {
                    // Setup: move up, place figures in the destruction row
                    var target = src.GetWithOffset(PositionConstants.U1);
                    var destroy1 = target.GetWithOffset((short)(-1 + 1 * PositionConstants.YOffset));
                    var destroy2 = target.GetWithOffset((short)(0 + 1 * PositionConstants.YOffset));
                    var destroy3 = target.GetWithOffset((short)(1 + 1 * PositionConstants.YOffset));

                    if (destroy1 != -1) b[destroy1] = Figure.Peasant | Figure.IsBlack;
                    if (destroy2 != -1) b[destroy2] = Figure.Archer | Figure.IsBlack;
                    if (destroy3 != -1) b[destroy3] = Figure.Knight | Figure.IsBlack;
                },
                src,
                warhammer,
                PositionConstants.U1,
                FigureActionType.WarhammerMove
            ),

            TestUtils.RunExecuteActionScenario(
                "Down movement destroys lower row", b =>
                {
                    // Setup: move down, place figures in the destruction row
                    var target = src.GetWithOffset(PositionConstants.D1);
                    var destroy1 = target.GetWithOffset((short)(-1 + -1 * PositionConstants.YOffset));
                    var destroy2 = target.GetWithOffset((short)(0 + -1 * PositionConstants.YOffset));
                    var destroy3 = target.GetWithOffset((short)(1 + -1 * PositionConstants.YOffset));

                    if (destroy1 != -1) b[destroy1] = Figure.Peasant | Figure.IsBlack;
                    if (destroy2 != -1) b[destroy2] = Figure.Archer | Figure.IsBlack;
                    if (destroy3 != -1) b[destroy3] = Figure.Knight | Figure.IsBlack;
                },
                src,
                warhammer,
                PositionConstants.D1,
                FigureActionType.WarhammerMove
            ),

            TestUtils.RunExecuteActionScenario(
                "Diagonal movement - no destruction", b =>
                {
                    // Setup: diagonal move should not trigger destruction
                    var target = src.GetWithOffset(PositionConstants.U1R1);
                    var destroy1 = target.GetWithOffset((short)(1 + 0 * PositionConstants.YOffset));
                    if (destroy1 != -1) b[destroy1] = Figure.Peasant | Figure.IsBlack; // Should not be destroyed
                },
                src,
                warhammer,
                PositionConstants.U1R1,
                FigureActionType.WarhammerMove
            )
        });
    }
}