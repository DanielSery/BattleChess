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
    public Task GetPossibleActions_Verify()
    {
        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* no adjacent enemies, empty board */ },
                27, // d4
                Figure.Catapult | Figure.IsWhite,
                Catapult.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Enemy adjacent suppress",
                b =>
                {
                    // Any adjacent enemy in queen directions suppresses all actions
                    var r = 27 /* d4 */.GetWithOffset(PositionConstants.R);
                    if (r != -1) b[r] = Figure.Peasant | Figure.IsBlack;
                },
                27, // d4
                Figure.Catapult | Figure.IsWhite,
                Catapult.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Mix possible and actual",
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
                    var t0 = 27 /* d4 */.GetWithOffset(rels[0]); // enemy -> RangedAttack
                    var t1 = 27 /* d4 */.GetWithOffset(rels[1]); // friendly -> PossibleRangedAttack
                    var t2 = 27 /* d4 */.GetWithOffset(rels[2]); // wall -> PossibleRangedAttack
                    if (t0 != -1) b[t0] = Figure.Archer | Figure.IsBlack;
                    if (t1 != -1) b[t1] = Figure.LegionarySword | Figure.IsWhite;
                    if (t2 != -1) b[t2] = Figure.Wall;
                    // leave others empty -> PossibleRangedAttack
                },
                27, // d4
                Figure.Catapult | Figure.IsWhite,
                Catapult.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                _ => { },
                0, // a1
                Figure.Catapult | Figure.IsWhite,
                Catapult.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case H8",
                _ => { },
                63, // h8
                Figure.Catapult | Figure.IsBlack,
                Catapult.GetPossibleActions),

            TestUtils.RunExecuteActionScenario(
                "Execute ranged attack",
                b =>
                {
                    var dst = 27 /* d4 */.GetWithOffset((short)(unchecked((byte)+0) - 3*PositionConstants.YOffset));
                    if (dst != -1) b[dst] = Figure.Knight | Figure.IsBlack;
                },
                27, // d4
                Figure.Catapult | Figure.IsWhite,
                (short)(unchecked((byte)+0) - 3*PositionConstants.YOffset),
                FigureActionType.RangedAttack)
        });
    }
}