using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class BattleAxeTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure axe = Figure.BattleAxe | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* nothing */ },
                src,
                axe,
                BattleAxe.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Blocked diagonals", b =>
                {
                    var ur = src.GetWithOffset(PositionConstants.UR);
                    var ul = src.GetWithOffset(PositionConstants.UL);
                    var dr = src.GetWithOffset(PositionConstants.DR);
                    var dl = src.GetWithOffset(PositionConstants.DL);
                    if (ul != -1) b[ul] = Figure.Wall; // not walkable
                    if (dr != -1) b[dr] = Figure.Peasant | Figure.IsWhite; // friendly – not walkable
                    if (dl != -1) b[dl] = Figure.Peasant | Figure.IsBlack; // enemy – not walkable
                    // leave UR empty if in-bounds
                    if (ur != -1) b[ur] = Figure.Empty;
                },
                src,
                axe,
                BattleAxe.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1", _ => { },
                0, // a1
                axe,
                BattleAxe.GetPossibleActions)
        });
    }

    [Fact]
    public Task ExecuteMove_Verify()
    {
        const int src = 27; // d4
        const Figure axe = Figure.BattleAxe | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunExecuteActionScenario(
                "Move UR destroys quadrant", b =>
                {
                    var dst = src.GetWithOffset(PositionConstants.UR);
                    if (dst == -1) return;
                    var ur = dst.GetWithOffset(PositionConstants.UR);
                    var u  = dst.GetWithOffset(PositionConstants.U);
                    var r  = dst.GetWithOffset(PositionConstants.R);
                    var l  = dst.GetWithOffset(PositionConstants.L); // should survive
                    if (ur != -1) b[ur] = Figure.Trader | Figure.IsBlack;
                    if (u  != -1) b[u]  = Figure.Archer | Figure.IsWhite;
                    if (r  != -1) b[r]  = Figure.Wall;
                    if (l  != -1) b[l]  = Figure.Knight | Figure.IsBlack; // should remain
                },
                src,
                axe,
                PositionConstants.UR,
                FigureActionType.BattleAxeMove),

            TestUtils.RunExecuteActionScenario(
                "Move UL destroys quadrant", b =>
                {
                    var dst = src.GetWithOffset(PositionConstants.UL);
                    if (dst == -1) return;
                    var ul = dst.GetWithOffset(PositionConstants.UL);
                    var u  = dst.GetWithOffset(PositionConstants.U);
                    var l  = dst.GetWithOffset(PositionConstants.L);
                    var r  = dst.GetWithOffset(PositionConstants.R); // should survive
                    if (ul != -1) b[ul] = Figure.Trader | Figure.IsBlack;
                    if (u  != -1) b[u]  = Figure.Archer | Figure.IsWhite;
                    if (l  != -1) b[l]  = Figure.Wall;
                    if (r  != -1) b[r]  = Figure.Knight | Figure.IsBlack; // should remain
                },
                src,
                axe,
                PositionConstants.UL,
                FigureActionType.BattleAxeMove),

            TestUtils.RunExecuteActionScenario(
                "Move DR destroys quadrant", b =>
                {
                    var dst = src.GetWithOffset(PositionConstants.DR);
                    if (dst == -1) return;
                    var dr = dst.GetWithOffset(PositionConstants.DR);
                    var d  = dst.GetWithOffset(PositionConstants.D);
                    var r  = dst.GetWithOffset(PositionConstants.R);
                    var u  = dst.GetWithOffset(PositionConstants.U); // should survive
                    if (dr != -1) b[dr] = Figure.Trader | Figure.IsBlack;
                    if (d  != -1) b[d]  = Figure.Archer | Figure.IsWhite;
                    if (r  != -1) b[r]  = Figure.Wall;
                    if (u  != -1) b[u]  = Figure.Knight | Figure.IsBlack; // should remain
                },
                src,
                axe,
                PositionConstants.DR,
                FigureActionType.BattleAxeMove),

            TestUtils.RunExecuteActionScenario(
                "Move DL destroys quadrant", b =>
                {
                    var dst = src.GetWithOffset(PositionConstants.DL);
                    if (dst == -1) return;
                    var dl = dst.GetWithOffset(PositionConstants.DL);
                    var d  = dst.GetWithOffset(PositionConstants.D);
                    var l  = dst.GetWithOffset(PositionConstants.L);
                    var r  = dst.GetWithOffset(PositionConstants.R); // should survive
                    if (dl != -1) b[dl] = Figure.Trader | Figure.IsBlack;
                    if (d  != -1) b[d]  = Figure.Archer | Figure.IsWhite;
                    if (l  != -1) b[l]  = Figure.Wall;
                    if (r  != -1) b[r]  = Figure.Knight | Figure.IsBlack; // should remain
                },
                src,
                axe,
                PositionConstants.DL,
                FigureActionType.BattleAxeMove)
        });
    }
}
