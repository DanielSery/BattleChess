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
                    var ur = src.GetWithOffset(PositionConstants.U1R1);
                    var ul = src.GetWithOffset(PositionConstants.U1L1);
                    var dr = src.GetWithOffset(PositionConstants.D1R1);
                    var dl = src.GetWithOffset(PositionConstants.D1L1);
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
                    var dst = src.GetWithOffset(PositionConstants.U1R1);
                    if (dst == -1) return;
                    var ur = dst.GetWithOffset(PositionConstants.U1R1);
                    var u  = dst.GetWithOffset(PositionConstants.U1);
                    var r  = dst.GetWithOffset(PositionConstants.R1);
                    var l  = dst.GetWithOffset(PositionConstants.L1); // should survive
                    if (ur != -1) b[ur] = Figure.Trader | Figure.IsBlack;
                    if (u  != -1) b[u]  = Figure.Archer | Figure.IsWhite;
                    if (r  != -1) b[r]  = Figure.Wall;
                    if (l  != -1) b[l]  = Figure.Knight | Figure.IsBlack; // should remain
                },
                src,
                axe,
                PositionConstants.U1R1,
                FigureActionType.BattleAxeMove),

            TestUtils.RunExecuteActionScenario(
                "Move UL destroys quadrant", b =>
                {
                    var dst = src.GetWithOffset(PositionConstants.U1L1);
                    if (dst == -1) return;
                    var ul = dst.GetWithOffset(PositionConstants.U1L1);
                    var u  = dst.GetWithOffset(PositionConstants.U1);
                    var l  = dst.GetWithOffset(PositionConstants.L1);
                    var r  = dst.GetWithOffset(PositionConstants.R1); // should survive
                    if (ul != -1) b[ul] = Figure.Trader | Figure.IsBlack;
                    if (u  != -1) b[u]  = Figure.Archer | Figure.IsWhite;
                    if (l  != -1) b[l]  = Figure.Wall;
                    if (r  != -1) b[r]  = Figure.Knight | Figure.IsBlack; // should remain
                },
                src,
                axe,
                PositionConstants.U1L1,
                FigureActionType.BattleAxeMove),

            TestUtils.RunExecuteActionScenario(
                "Move DR destroys quadrant", b =>
                {
                    var dst = src.GetWithOffset(PositionConstants.D1R1);
                    if (dst == -1) return;
                    var dr = dst.GetWithOffset(PositionConstants.D1R1);
                    var d  = dst.GetWithOffset(PositionConstants.D1);
                    var r  = dst.GetWithOffset(PositionConstants.R1);
                    var u  = dst.GetWithOffset(PositionConstants.U1); // should survive
                    if (dr != -1) b[dr] = Figure.Trader | Figure.IsBlack;
                    if (d  != -1) b[d]  = Figure.Archer | Figure.IsWhite;
                    if (r  != -1) b[r]  = Figure.Wall;
                    if (u  != -1) b[u]  = Figure.Knight | Figure.IsBlack; // should remain
                },
                src,
                axe,
                PositionConstants.D1R1,
                FigureActionType.BattleAxeMove),

            TestUtils.RunExecuteActionScenario(
                "Move DL destroys quadrant", b =>
                {
                    var dst = src.GetWithOffset(PositionConstants.D1L1);
                    if (dst == -1) return;
                    var dl = dst.GetWithOffset(PositionConstants.D1L1);
                    var d  = dst.GetWithOffset(PositionConstants.D1);
                    var l  = dst.GetWithOffset(PositionConstants.L1);
                    var r  = dst.GetWithOffset(PositionConstants.R1); // should survive
                    if (dl != -1) b[dl] = Figure.Trader | Figure.IsBlack;
                    if (d  != -1) b[d]  = Figure.Archer | Figure.IsWhite;
                    if (l  != -1) b[l]  = Figure.Wall;
                    if (r  != -1) b[r]  = Figure.Knight | Figure.IsBlack; // should remain
                },
                src,
                axe,
                PositionConstants.D1L1,
                FigureActionType.BattleAxeMove)
        });
    }
}
