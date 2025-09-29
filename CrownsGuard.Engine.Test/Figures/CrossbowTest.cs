using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class CrossbowTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_White_Verify()
    {
        const int src = 27; // d4
        const Figure crossbow = Figure.Crossbow | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "AllEmpty_White",
            _ => { /* empty around */ },
            src,
            crossbow,
            Crossbow.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EnemyAdjacentSuppress_Verify()
    {
        const int src = 27; // d4
        const Figure crossbow = Figure.Crossbow | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "EnemyAdjacentSuppress",
            b =>
            {
                // Any adjacent enemy in queen directions suppresses all actions
                var ur = src.GetWithOffset(PositionConstants.UR);
                if (ur != -1) b[ur] = Figure.Peasant | Figure.IsBlack;
            },
            src,
            crossbow,
            Crossbow.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_MixPossibleAndActual_White_Verify()
    {
        const int src = 27; // d4
        const Figure crossbow = Figure.Crossbow | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "MixPossibleAndActual_White",
            b =>
            {
                // Diagonals up to 3 tiles
                // UL: enemy at distance 2
                var ul1 = src.GetWithOffset(PositionConstants.UL);
                var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.UL);
                if (ul2 != -1) b[ul2] = Figure.Peasant | Figure.IsBlack;

                // UR: friendly at distance 1 (blocks and should add Possible at that tile and break)
                var ur1 = src.GetWithOffset(PositionConstants.UR);
                if (ur1 != -1) b[ur1] = Figure.LegionarySword | Figure.IsWhite;

                // DR: wall at distance 3 (should add Possible at 1,2 and at 3 then break)
                var dr1 = src.GetWithOffset(PositionConstants.DR);
                var dr2 = dr1 == -1 ? -1 : dr1.GetWithOffset(PositionConstants.DR);
                var dr3 = dr2 == -1 ? -1 : dr2.GetWithOffset(PositionConstants.DR);
                if (dr3 != -1) b[dr3] = Figure.Wall;

                // DL: enemy at distance 2 (ensure adjacent diagonal empty to avoid suppression)
                var dl1 = src.GetWithOffset(PositionConstants.DL);
                var dl2 = dl1 == -1 ? -1 : dl1.GetWithOffset(PositionConstants.DL);
                if (dl2 != -1) b[dl2] = Figure.Knight | Figure.IsBlack;

                // Rook neighbors: make Left non-walkable with friendly, others empty
                var l = src.GetWithOffset(PositionConstants.L);
                if (l != -1) b[l] = Figure.Spearman | Figure.IsWhite; // no move to left
            },
            src,
            crossbow,
            Crossbow.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_White_Verify()
    {
        const int src = 0; // a1
        const Figure crossbow = Figure.Crossbow | Figure.IsWhite;
        return Verify(TestUtils.RunGetActionsScenario(
            "EdgeCase_A1_White",
            _ => { },
            src,
            crossbow,
            Crossbow.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_H8_Black_Verify()
    {
        const int src = 63; // h8
        const Figure crossbow = Figure.Crossbow | Figure.IsBlack;
        return Verify(TestUtils.RunGetActionsScenario(
            "EdgeCase_H8_Black",
            _ => { },
            src,
            crossbow,
            Crossbow.GetPossibleActions
        ));
    }

    [Fact]
    public Task ExecuteRangedAttack_White_Verify()
    {
        const int src = 27; // d4
        const Figure crossbow = Figure.Crossbow | Figure.IsWhite;

        // pick a diagonal distance-2 target: UR + UR
        short relative = (short)(PositionConstants.UR + PositionConstants.UR);

        return Verify(TestUtils.RunExecuteActionScenario(
            "ExecuteRangedAttack_White",
            b =>
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
