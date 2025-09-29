using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class CannonTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Verify()
    {
        const int src = 27; // d4
        const Figure cannon = Figure.Cannon | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "AllEmpty",
            _ => { /* no blockers, no adjacent enemies */ },
            src,
            cannon,
            Cannon.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EnemyAdjacentSuppressAll_Verify()
    {
        const int src = 27; // d4
        const Figure cannon = Figure.Cannon | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "EnemyAdjacentSuppressAll",
            b =>
            {
                var up = src.GetWithOffset(PositionConstants.U);
                if (up != -1) b[up] = Figure.Peasant | Figure.IsBlack; // any adjacent enemy suppresses all actions
            },
            src,
            cannon,
            Cannon.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_MixPossibleAndActual_Verify()
    {
        const int src = 27; // d4
        const Figure cannon = Figure.Cannon | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "MixPossibleAndActual",
            b =>
            {
                // Place pieces exactly at 2-4 squares forward (white moves up)
                var u1 = src.GetWithOffset(PositionConstants.U);
                var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U);
                var u4 = u3 == -1 ? -1 : u3.GetWithOffset(PositionConstants.U);
                if (u2 != -1) b[u2] = Figure.Peasant | Figure.IsBlack; // enemy -> CannonAttack
                if (u3 != -1) b[u3] = Figure.Peasant | Figure.IsWhite; // friendly -> PossibleCannonAttack
                if (u4 != -1) b[u4] = Figure.Wall; // wall -> PossibleCannonAttack
            },
            src,
            cannon,
            Cannon.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_White_Verify()
    {
        const int src = 0; // a1
        const Figure cannon = Figure.Cannon | Figure.IsWhite;
        return Verify(TestUtils.RunGetActionsScenario(
            "EdgeCase_A1_White",
            _ => { },
            src,
            cannon,
            Cannon.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_H8_Black_Verify()
    {
        const int src = 63; // h8
        const Figure cannon = Figure.Cannon | Figure.IsBlack;
        return Verify(TestUtils.RunGetActionsScenario(
            "EdgeCase_H8_Black",
            _ => { },
            src,
            cannon,
            Cannon.GetPossibleActions
        ));
    }

    [Fact]
    public Task ExecuteAttack_White_KillsAt2to4Forward_Verify()
    {
        const int src = 27; // d4
        const Figure cannon = Figure.Cannon | Figure.IsWhite;

        return Verify(TestUtils.RunExecuteActionScenario(
            "ExecuteAttack_White_KillsAt2to4Forward",
            b =>
            {
                // Enemies at 2 and 3 up; empty at 4 up
                var u1 = src.GetWithOffset(PositionConstants.U);
                var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U);
                var u3 = u2 == -1 ? -1 : u2.GetWithOffset(PositionConstants.U);
                var u4 = u3 == -1 ? -1 : u3.GetWithOffset(PositionConstants.U);
                if (u2 != -1) b[u2] = Figure.Knight | Figure.IsBlack;
                if (u3 != -1) b[u3] = Figure.Archer | Figure.IsBlack;
                // leave u4 empty
            },
            src,
            cannon,
            // Use relative of two steps up to target a valid CannonAttack square
            (short)(unchecked((byte)+0) - 2*PositionConstants.YOffset),
            FigureActionType.CannonAttack
        ));
    }

    [Fact]
    public Task ExecuteAttack_Black_KillsAt2to4Forward_Verify()
    {
        const int src = 36; // e5 (somewhere central)
        const Figure cannon = Figure.Cannon | Figure.IsBlack;

        return Verify(TestUtils.RunExecuteActionScenario(
            "ExecuteAttack_Black_KillsAt2to4Forward",
            b =>
            {
                // For black, forward is down (+YOffset)
                var d1 = src.GetWithOffset(PositionConstants.D);
                var d2 = d1 == -1 ? -1 : d1.GetWithOffset(PositionConstants.D);
                var d3 = d2 == -1 ? -1 : d2.GetWithOffset(PositionConstants.D);
                var d4 = d3 == -1 ? -1 : d3.GetWithOffset(PositionConstants.D);
                if (d2 != -1) b[d2] = Figure.Trader | Figure.IsWhite;
                if (d4 != -1) b[d4] = Figure.Wall; // non-empty also gets destroyed
            },
            src,
            cannon,
            (short)(unchecked((byte)+0) + 2*PositionConstants.YOffset),
            FigureActionType.CannonAttack
        ));
    }
}
