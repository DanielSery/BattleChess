using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class CrossbowTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty - White",
                _ => { /* empty around */ },
                27, // d4
                Figure.Crossbow | Figure.IsWhite,
                Crossbow.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Enemy adjacent suppress - White",
                b =>
                {
                    // Any adjacent enemy in queen directions suppresses all actions
                    var ur = 27.GetWithOffset(PositionConstants.U1R1);
                    if (ur != -1) b[ur] = Figure.Peasant | Figure.IsBlack;
                },
                27, // d4
                Figure.Crossbow | Figure.IsWhite,
                Crossbow.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Mix possible and actual - White",
                b =>
                {
                    // Diagonals up to 3 tiles
                    // UL: enemy at distance 2
                    var ul1 = 27.GetWithOffset(PositionConstants.U1L1);
                    var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.U1L1);
                    if (ul2 != -1) b[ul2] = Figure.Peasant | Figure.IsBlack;

                    // UR: friendly at distance 1 (blocks and should add Possible at that tile and break)
                    var ur1 = 27.GetWithOffset(PositionConstants.U1R1);
                    if (ur1 != -1) b[ur1] = Figure.LegionarySword | Figure.IsWhite;

                    // DR: wall at distance 3 (should add Possible at 1,2 and at 3 then break)
                    var dr1 = 27.GetWithOffset(PositionConstants.D1R1);
                    var dr2 = dr1 == -1 ? -1 : dr1.GetWithOffset(PositionConstants.D1R1);
                    var dr3 = dr2 == -1 ? -1 : dr2.GetWithOffset(PositionConstants.D1R1);
                    if (dr3 != -1) b[dr3] = Figure.Wall;

                    // DL: enemy at distance 2 (ensure adjacent diagonal empty to avoid suppression)
                    var dl1 = 27.GetWithOffset(PositionConstants.D1L1);
                    var dl2 = dl1 == -1 ? -1 : dl1.GetWithOffset(PositionConstants.D1L1);
                    if (dl2 != -1) b[dl2] = Figure.Knight | Figure.IsBlack;

                    // Rook neighbors: make Left non-walkable with friendly, others empty
                    var l = 27.GetWithOffset(PositionConstants.L1);
                    if (l != -1) b[l] = Figure.Spearman | Figure.IsWhite; // no move to left
                },
                27, // d4
                Figure.Crossbow | Figure.IsWhite,
                Crossbow.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1 - White",
                _ => { },
                0, // a1
                Figure.Crossbow | Figure.IsWhite,
                Crossbow.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case H8 - Black",
                _ => { },
                63, // h8
                Figure.Crossbow | Figure.IsBlack,
                Crossbow.GetPossibleActions)
        });
    }

    [Fact]
    public Task ExecuteRangedAttack_Verify()
    {
        const int src = 27; // d4
        const Figure crossbow = Figure.Crossbow | Figure.IsWhite;

        // pick a diagonal distance-2 target: UR + UR
        short relative = (short)(PositionConstants.U1R1 + PositionConstants.U1R1);

        return Verify(TestUtils.RunExecuteActionScenario("Simple", b =>
            {
                var dst = src.GetWithOffset(relative);
                if (dst != -1) b[dst] = Figure.Knight | Figure.IsBlack;
            },
            src,
            crossbow,
            relative,
            FigureActionType.RangedAttack
        ));
    }
}
