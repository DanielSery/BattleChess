using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class CatapultTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_White_Verify()
    {
        const int src = 27; // d4
        const Figure catapult = Figure.Catapult | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "AllEmpty_White",
            _ => { /* no adjacent enemies, empty board */ },
            src,
            catapult,
            Catapult.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EnemyAdjacentSuppress_Verify()
    {
        const int src = 27; // d4
        const Figure catapult = Figure.Catapult | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "EnemyAdjacentSuppress",
            b =>
            {
                // Any adjacent enemy in queen directions suppresses all actions
                var r = src.GetWithOffset(PositionConstants.R);
                if (r != -1) b[r] = Figure.Peasant | Figure.IsBlack;
            },
            src,
            catapult,
            Catapult.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_MixPossibleAndActual_White_Verify()
    {
        const int src = 27; // d4
        const Figure catapult = Figure.Catapult | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(
            "MixPossibleAndActual_White",
            b =>
            {
                // White attacks 2 and 3 tiles forward (up, -YOffset) with lateral offsets
                var rels = new short[]
                {
                    (short)(unchecked((byte)-1) - 2*PositionConstants.YOffset),
                    (short)(unchecked((byte)+1) - 2*PositionConstants.YOffset),
                    (short)(unchecked((byte)-2) - 3*PositionConstants.YOffset),
                    (short)(unchecked((byte)+0) - 3*PositionConstants.YOffset),
                    (short)(unchecked((byte)+2) - 3*PositionConstants.YOffset),
                };

                // Place different kinds of targets on some of them
                var t0 = src.GetWithOffset(rels[0]); // enemy -> RangedAttack
                var t1 = src.GetWithOffset(rels[1]); // friendly -> PossibleRangedAttack
                var t2 = src.GetWithOffset(rels[2]); // wall -> PossibleRangedAttack
                if (t0 != -1) b[t0] = Figure.Archer | Figure.IsBlack;
                if (t1 != -1) b[t1] = Figure.LegionarySword | Figure.IsWhite;
                if (t2 != -1) b[t2] = Figure.Wall;
                // leave others empty -> PossibleRangedAttack
            },
            src,
            catapult,
            Catapult.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_White_Verify()
    {
        const int src = 0; // a1
        const Figure catapult = Figure.Catapult | Figure.IsWhite;
        return Verify(TestUtils.RunGetActionsScenario(
            "EdgeCase_A1_White",
            _ => { },
            src,
            catapult,
            Catapult.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_H8_Black_Verify()
    {
        const int src = 63; // h8
        const Figure catapult = Figure.Catapult | Figure.IsBlack;
        return Verify(TestUtils.RunGetActionsScenario(
            "EdgeCase_H8_Black",
            _ => { },
            src,
            catapult,
            Catapult.GetPossibleActions
        ));
    }

    [Fact]
    public Task ExecuteRangedAttack_White_Verify()
    {
        const int src = 27; // d4
        const Figure catapult = Figure.Catapult | Figure.IsWhite;

        // pick the central far target: 0 - 3*YOffset
        short relative = (short)(unchecked((byte)+0) - 3*PositionConstants.YOffset);

        return Verify(TestUtils.RunExecuteActionScenario(
            "ExecuteRangedAttack_White",
            b =>
            {
                var dst = src.GetWithOffset(relative);
                if (dst != -1) b[dst] = Figure.Knight | Figure.IsBlack;
            },
            src,
            catapult,
            relative,
            FigureActionType.RangedAttack
        ));
    }
}