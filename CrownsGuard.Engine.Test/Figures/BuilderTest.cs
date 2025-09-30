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
                    var u = src.GetWithOffset(PositionConstants.U);
                    var r = src.GetWithOffset(PositionConstants.R);
                    var d = src.GetWithOffset(PositionConstants.D);
                    var l = src.GetWithOffset(PositionConstants.L);
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
                    var u = src.GetWithOffset(PositionConstants.U);
                    if (u != -1) b[u] = Figure.Empty;
                    // Right friendly (non-empty non-wall) -> no action on rook direction
                    var r = src.GetWithOffset(PositionConstants.R);
                    if (r != -1) b[r] = Figure.LegionarySword | Figure.IsWhite;
                    // Down enemy (non-empty non-wall) -> no action on rook direction
                    var d = src.GetWithOffset(PositionConstants.D);
                    if (d != -1) b[d] = Figure.Peasant | Figure.IsBlack;
                    // Left wall -> MeleeAttack
                    var l = src.GetWithOffset(PositionConstants.L);
                    if (l != -1) b[l] = Figure.Wall;

                    // Diagonals: make two walkable and two blocked to verify move generation only on walkable
                    var ul = src.GetWithOffset(PositionConstants.UL);
                    var ur = src.GetWithOffset(PositionConstants.UR);
                    var dl = src.GetWithOffset(PositionConstants.DL);
                    var dr = src.GetWithOffset(PositionConstants.DR);
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
}
