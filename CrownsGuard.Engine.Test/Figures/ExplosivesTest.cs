using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class ExplosivesTest
{
    [Fact]
    public Task OnAttacked_FiguresAttackExplosives_CenterPositions()
    {
        const int explosivesPosition = 27; // d4 - center of board
        const Figure explosives = Figure.Explosives;

        return Verify(new List<object>
        {
            // Melee Attack Tests
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Empty board - peasant melee attacks explosives",
                b => b[explosivesPosition] = explosives,
                explosivesPosition.GetWithOffset(PositionConstants.U1), // Place attacker above explosives
                Figure.Peasant | Figure.IsWhite,
                PositionConstants.D1, // Attack down toward explosives
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Mixed figures melee attacking explosives",
                b =>
                {
                    // Place explosives at center
                    b[explosivesPosition] = explosives;

                    // Place various figures around explosives to attack it
                    var u1 = explosivesPosition.GetWithOffset(PositionConstants.U1);
                    var r1 = explosivesPosition.GetWithOffset(PositionConstants.R1);
                    var d1 = explosivesPosition.GetWithOffset(PositionConstants.D1);
                    var l1 = explosivesPosition.GetWithOffset(PositionConstants.L1);

                    if (u1 != -1) b[u1] = Figure.Peasant | Figure.IsWhite;
                    if (r1 != -1) b[r1] = Figure.Knight | Figure.IsBlack;
                    if (d1 != -1) b[d1] = Figure.Archer | Figure.IsWhite;
                    if (l1 != -1) b[l1] = Figure.Builder | Figure.IsBlack;
                },
                explosivesPosition.GetWithOffset(PositionConstants.U1), // Attack from above
                Figure.Peasant | Figure.IsWhite,
                PositionConstants.D1, // Attack down toward explosives
                FigureActionType.MeleeAttack
            ),

            // Melee Pierce Attack Tests
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Empty board - knight melee pierce attacks explosives",
                b => b[explosivesPosition] = explosives,
                explosivesPosition.GetWithOffset(PositionConstants.U2), // Place attacker two spaces above explosives
                Figure.Knight | Figure.IsWhite,
                PositionConstants.D2, // Attack down toward explosives (pierce through empty space)
                FigureActionType.MeleePierceAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Knight melee pierce attacking explosives with figures in path",
                b =>
                {
                    // Place explosives at center
                    b[explosivesPosition] = explosives;

                    // Place a figure between knight and explosives for piercing
                    var u1 = explosivesPosition.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Peasant | Figure.IsBlack; // Will be pierced through

                    // Place knight two spaces away
                    var u2 = explosivesPosition.GetWithOffset(PositionConstants.U2);
                    if (u2 != -1) b[u2] = Figure.Knight | Figure.IsWhite;
                },
                explosivesPosition.GetWithOffset(PositionConstants.U2), // Attack from two spaces above
                Figure.Knight | Figure.IsWhite,
                PositionConstants.D2, // Attack down toward explosives
                FigureActionType.MeleePierceAttack
            ),

            // Ranged Attack Tests
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Empty board - archer ranged attacks explosives",
                b => b[explosivesPosition] = explosives,
                explosivesPosition.GetWithOffset(PositionConstants.U3), // Place archer three spaces above explosives
                Figure.Archer | Figure.IsWhite,
                PositionConstants.D3, // Attack down toward explosives from range
                FigureActionType.RangedAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Crossbow ranged attacking explosives",
                b =>
                {
                    // Place explosives at center
                    b[explosivesPosition] = explosives;

                    // Place crossbow three spaces away
                    var u3 = explosivesPosition.GetWithOffset(PositionConstants.U3);
                    if (u3 != -1) b[u3] = Figure.Crossbow | Figure.IsWhite;
                },
                explosivesPosition.GetWithOffset(PositionConstants.U3), // Attack from three spaces above
                Figure.Crossbow | Figure.IsWhite,
                PositionConstants.D3, // Attack down toward explosives
                FigureActionType.RangedAttack
            ),

            // Cannon Attack Tests
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Empty board - cannon attacks explosives",
                b => b[explosivesPosition] = explosives,
                explosivesPosition.GetWithOffset(PositionConstants.U4), // Place cannon four spaces above explosives
                Figure.Cannon | Figure.IsWhite,
                PositionConstants.D4, // Attack down toward explosives from distance
                FigureActionType.CannonAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "Cannon attacking explosives with figures in path",
                b =>
                {
                    // Place explosives at center
                    b[explosivesPosition] = explosives;

                    // Place some figures between cannon and explosives
                    var u2 = explosivesPosition.GetWithOffset(PositionConstants.U2);
                    if (u2 != -1) b[u2] = Figure.Peasant | Figure.IsBlack; // Will be hit by cannon

                    // Place cannon four spaces away
                    var u4 = explosivesPosition.GetWithOffset(PositionConstants.U4);
                    if (u4 != -1) b[u4] = Figure.Cannon | Figure.IsWhite;
                },
                explosivesPosition.GetWithOffset(PositionConstants.U4), // Attack from four spaces above
                Figure.Cannon | Figure.IsWhite,
                PositionConstants.D4, // Attack down toward explosives
                FigureActionType.CannonAttack
            )
        });
    }

    [Fact]
    public Task OnAttacked_FiguresAttackExplosives_CornerPositions()
    {
        const Figure explosives = Figure.Explosives;

        return Verify(new List<object>
        {
            // Melee Attack Tests - Corner Positions
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A1 corner - peasant melee attacks explosives from right",
                b => b[0] = explosives,
                1, // b1 - attack right to A1
                Figure.Peasant | Figure.IsWhite,
                PositionConstants.L1, // Attack left toward explosives
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "H1 corner - archer melee attacks explosives from left",
                b => b[7] = explosives,
                6, // g1 - attack right to H1
                Figure.Archer | Figure.IsBlack,
                PositionConstants.R1, // Attack right toward explosives
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A8 corner - knight melee attacks explosives from right",
                b => b[56] = explosives,
                57, // b8 - attack left to A8
                Figure.Knight | Figure.IsWhite,
                PositionConstants.L1, // Attack left toward explosives
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "H8 corner - builder melee attacks explosives from left",
                b => b[63] = explosives,
                62, // g8 - attack right to H8
                Figure.Builder | Figure.IsBlack,
                PositionConstants.R1, // Attack right toward explosives
                FigureActionType.MeleeAttack
            ),

            // Melee Pierce Attack Tests - Corner Positions
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A1 corner - chinese melee pierce attacks explosives",
                b => b[0] = explosives,
                2, // c1 - attack left to A1 (pierce through b1)
                Figure.Chinese | Figure.IsWhite,
                PositionConstants.L2, // Attack left toward explosives
                FigureActionType.MeleePierceAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "H1 corner - samurai melee pierce attacks explosives",
                b => b[7] = explosives,
                5, // f1 - attack right to H1 (pierce through g1)
                Figure.Samurai | Figure.IsBlack,
                PositionConstants.R2, // Attack right toward explosives
                FigureActionType.MeleePierceAttack
            ),

            // Ranged Attack Tests - Corner Positions
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A1 corner - ranger ranged attacks explosives",
                b => b[0] = explosives,
                3, // d1 - attack left to A1 from range
                Figure.Ranger | Figure.IsWhite,
                PositionConstants.L3, // Attack left toward explosives
                FigureActionType.RangedAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "H8 corner - musketeer ranged attacks explosives",
                b => b[63] = explosives,
                60, // e8 - attack right to H8 from range
                Figure.Musketeer | Figure.IsBlack,
                PositionConstants.R3, // Attack right toward explosives
                FigureActionType.RangedAttack
            ),

            // Cannon Attack Tests - Corner Positions
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A1 corner - cannon attacks explosives",
                b => b[0] = explosives,
                4, // e1 - attack left to A1 from distance
                Figure.Cannon | Figure.IsWhite,
                PositionConstants.L4, // Attack left toward explosives
                FigureActionType.CannonAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "H8 corner - cannon attacks explosives",
                b => b[63] = explosives,
                59, // d8 - attack right to H8 from distance
                Figure.Cannon | Figure.IsBlack,
                PositionConstants.R4, // Attack right toward explosives
                FigureActionType.CannonAttack
            )
        });
    }

    [Fact]
    public Task OnAttacked_FiguresAttackExplosives_SideEdgePositions()
    {
        const Figure explosives = Figure.Explosives;

        return Verify(new List<object>
        {
            // Melee Attack Tests - Side Edge Positions
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "B1 side edge - peasant melee attacks explosives from right",
                b => b[1] = explosives,
                2, // c1 - attack left to B1
                Figure.Peasant | Figure.IsWhite,
                PositionConstants.L1, // Attack left toward explosives
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "G1 side edge - archer melee attacks explosives from right",
                b => b[6] = explosives,
                5, // f1 - attack right to G1
                Figure.Archer | Figure.IsBlack,
                PositionConstants.R1, // Attack right toward explosives
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A4 side edge - knight melee attacks explosives from below",
                b => b[24] = explosives,
                32, // a5 - attack up to A4
                Figure.Knight | Figure.IsWhite,
                PositionConstants.U1, // Attack up toward explosives
                FigureActionType.MeleeAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "H4 side edge - builder melee attacks explosives from below",
                b => b[31] = explosives,
                39, // h5 - attack up to H4
                Figure.Builder | Figure.IsBlack,
                PositionConstants.U1, // Attack up toward explosives
                FigureActionType.MeleeAttack
            ),

            // Melee Pierce Attack Tests - Side Edge Positions
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "B1 side edge - nordguard melee pierce attacks explosives",
                b => b[1] = explosives,
                3, // d1 - attack left to B1 (pierce through c1)
                Figure.Nordguard | Figure.IsWhite,
                PositionConstants.L2, // Attack left toward explosives
                FigureActionType.MeleePierceAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "A4 side edge - blade melee pierce attacks explosives",
                b => b[24] = explosives,
                32, // a5 - attack up to A4 (pierce through nothing, direct attack)
                Figure.Blade | Figure.IsWhite,
                PositionConstants.U1, // Attack up toward explosives (1 space)
                FigureActionType.MeleePierceAttack
            ),

            // Ranged Attack Tests - Side Edge Positions
            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "B1 side edge - japan archer ranged attacks explosives",
                b => b[1] = explosives,
                4, // e1 - attack left to B1 from range
                Figure.JapanArcher | Figure.IsWhite,
                PositionConstants.L3, // Attack left toward explosives
                FigureActionType.RangedAttack
            ),

            TestUtils.RunExecuteActionScenarioWithEventHandling(
                "H4 side edge - catapult ranged attacks explosives",
                b => b[31] = explosives,
                39, // h5 - attack up to H4 from range (1 space up: h5->h4)
                Figure.Catapult | Figure.IsBlack,
                PositionConstants.U1, // Attack up toward explosives (1 space)
                FigureActionType.RangedAttack
            ),


        });
    }
}