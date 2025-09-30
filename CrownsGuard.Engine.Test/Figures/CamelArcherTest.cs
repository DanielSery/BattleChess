using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class CamelArcherTest
{
    [Fact]
    public Task GetPossibleActions_Verify()
    {
        const int src = 27; // d4
        const Figure camelArcher = Figure.CamelArcher | Figure.IsWhite;

        return Verify(new List<object>
        {
            TestUtils.RunGetActionsScenario(
                "All empty",
                _ => { /* empty around */ },
                src,
                camelArcher,
                CamelArcher.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Blocking and enemy within rook",
                b =>
                {
                    // Rook lines setup for melee path
                    // Left: empty, empty, then enemy at distance 3
                    var l1 = src.GetWithOffset(PositionConstants.L1);
                    var l2 = l1 == -1 ? -1 : l1.GetWithOffset(PositionConstants.L1);
                    var l3 = l2 == -1 ? -1 : l2.GetWithOffset(PositionConstants.L1);
                    if (l1 != -1) b[l1] = Figure.Empty; // walkable -> PossibleMeleeAttack
                    if (l2 != -1) b[l2] = Figure.Empty; // walkable -> PossibleMeleeAttack
                    if (l3 != -1) b[l3] = Figure.Peasant | Figure.IsBlack; // enemy -> MeleeAttack

                    // Up: empty, then friendly at distance 2 (blocks further)
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    var u2 = u1 == -1 ? -1 : u1.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Empty; // walkable -> PossibleMeleeAttack
                    if (u2 != -1) b[u2] = Figure.LegionarySword | Figure.IsWhite; // friendly -> PossibleMeleeAttack then stop

                    // Right: wall immediately (non-walkable blocker)
                    var r1 = src.GetWithOffset(PositionConstants.R1);
                    if (r1 != -1) b[r1] = Figure.Wall; // -> PossibleMeleeAttack then stop

                    // Down: leave empty to edge -> all squares become PossibleMeleeAttack

                    // Diagonal movement blockers for bishop-like moves
                    // Up-Left: wall immediately blocks any move
                    var ul1 = src.GetWithOffset(PositionConstants.U1L1);
                    if (ul1 != -1) b[ul1] = Figure.Wall;

                    // Up-Right: empty then friendly at distance 2 blocks further
                    var ur1 = src.GetWithOffset(PositionConstants.U1R1);
                    var ur2 = ur1 == -1 ? -1 : ur1.GetWithOffset(PositionConstants.U1R1);
                    if (ur1 != -1) b[ur1] = Figure.Empty; // walkable move
                    if (ur2 != -1) b[ur2] = Figure.Peasant | Figure.IsWhite; // blocks further

                    // Down-Left and Down-Right left empty to edge
                },
                src,
                camelArcher,
                CamelArcher.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Enemy adjacent rook",
                b =>
                {
                    // Enemy immediately above -> should be MeleeAttack and stop in that direction
                    var u1 = src.GetWithOffset(PositionConstants.U1);
                    if (u1 != -1) b[u1] = Figure.Peasant | Figure.IsBlack;
                    // Place some diagonal blockers to ensure movement respects walkability
                    var dl1 = src.GetWithOffset(PositionConstants.D1L1);
                    if (dl1 != -1) b[dl1] = Figure.LegionarySword | Figure.IsWhite; // blocks diagonal move
                },
                src,
                camelArcher,
                CamelArcher.GetPossibleActions),

            TestUtils.RunGetActionsScenario(
                "Edge case A1",
                _ => { /* archer at edge via src override */ },
                0, // a1
                camelArcher,
                CamelArcher.GetPossibleActions)
        });
    }
}
