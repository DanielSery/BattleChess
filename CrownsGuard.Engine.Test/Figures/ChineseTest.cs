using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class ChineseTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Center_White_Verify()
    {
        const int src = 27; // d4
        const Figure chinese = Figure.Chinese | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ => { /* empty board */ },
            src,
            chinese,
            Chinese.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_MixedBlockingAndAttacks_Verify()
    {
        const int src = 27; // d4
        const Figure chinese = Figure.Chinese | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // UL: enemy at distance 2
                var ul1 = src.GetWithOffset(PositionConstants.UL);
                var ul2 = ul1 == -1 ? -1 : ul1.GetWithOffset(PositionConstants.UL);
                if (ul2 != -1) b[ul2] = Figure.Peasant | Figure.IsBlack;
                // UR: friendly at distance 1 (blocks and should add Possible at that tile and break)
                var ur1 = src.GetWithOffset(PositionConstants.UR);
                if (ur1 != -1) b[ur1] = Figure.LegionarySword | Figure.IsWhite;
                // DR: wall at distance 3 (should add Possible at 1,2 and Possible at 3 then break)
                var dr1 = src.GetWithOffset(PositionConstants.DR);
                var dr2 = dr1 == -1 ? -1 : dr1.GetWithOffset(PositionConstants.DR);
                var dr3 = dr2 == -1 ? -1 : dr2.GetWithOffset(PositionConstants.DR);
                if (dr3 != -1) b[dr3] = Figure.Wall;
                // DL: enemy at distance 1 (attack at 1, then possibles further)
                var dl1 = src.GetWithOffset(PositionConstants.DL);
                if (dl1 != -1) b[dl1] = Figure.Knight | Figure.IsBlack;

                // Rook neighbors: make Left non-walkable with friendly, others empty
                var l = src.GetWithOffset(PositionConstants.L);
                if (l != -1) b[l] = Figure.Spearman | Figure.IsWhite; // no move to left
                // Up, Right, Down remain empty (walkable)
            },
            src,
            chinese,
            Chinese.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_White_Verify()
    {
        const int src = 0; // a1
        const Figure chinese = Figure.Chinese | Figure.IsWhite;
        return Verify(TestUtils.RunGetActionsScenario(_ => { },
            src,
            chinese,
            Chinese.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_H8_Black_Verify()
    {
        const int src = 63; // h8
        const Figure chinese = Figure.Chinese | Figure.IsBlack;
        return Verify(TestUtils.RunGetActionsScenario(_ => { },
            src,
            chinese,
            Chinese.GetPossibleActions
        ));
    }

    [Fact]
    public Task ExecuteMeleePierceAttack_Diagonal_Distance2_Verify()
    {
        const int src = 27; // d4
        const Figure chinese = Figure.Chinese | Figure.IsWhite;
        // Use distance-2 diagonal (UR twice)
        short relative = (short)(PositionConstants.UR + PositionConstants.UR);

        return Verify(TestUtils.RunExecuteActionScenario(b =>
            {
                var dst = src.GetWithOffset(relative);
                if (dst != -1) b[dst] = Figure.Dogs | Figure.IsBlack;
            },
            src,
            chinese,
            relative,
            FigureActionType.MeleePierceAttack
        ));
    }
}
