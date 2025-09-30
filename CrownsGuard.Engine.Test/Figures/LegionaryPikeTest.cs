using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class LegionaryPikeTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Verify()
    {
        const int src = 27; // d4
        const Figure pike = Figure.LegionaryPike | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ => { /* empty around */ },
            src,
            pike,
            LegionaryPike.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_MixedBlockingAndAttacks_Verify()
    {
        const int src = 27; // d4
        const Figure pike = Figure.LegionaryPike | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                // Melee (white forward-diagonals): one enemy and one friendly adjacent
                var ul = src.GetWithOffset(PositionConstants.UL);
                var ur = src.GetWithOffset(PositionConstants.UR);
                if (ul != -1) b[ul] = Figure.Peasant | Figure.IsBlack; // enemy - melee attack possible
                if (ur != -1) b[ur] = Figure.LegionarySword | Figure.IsWhite; // friendly - only PossibleMeleeAttack

                // Pike ranged (two forward diagonals): one enemy and one non-attackable (friendly) to produce PossibleRangedAttack
                var uul = src.GetWithOffset(unchecked((byte)-1) - 2 * PositionConstants.YOffset);
                var uur = src.GetWithOffset(unchecked((byte)+1) - 2 * PositionConstants.YOffset);
                if (uul != -1) b[uul] = Figure.Peasant | Figure.IsBlack; // enemy - ranged attack
                if (uur != -1) b[uur] = Figure.LegionarySword | Figure.IsWhite; // friendly - only PossibleRangedAttack

                // Forward moves: place a blocker immediately ahead to block both single and double moves
                var u = src.GetWithOffset(PositionConstants.D); // for white forward is down (negative Y)
                // Note: in engine, white forward move uses -1 * YOffset (PositionConstants.D)
                if (u != -1) b[u] = Figure.Wall; // non-walkable blocker
            },
            src,
            pike,
            LegionaryPike.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_StartDoubleStep_WhiteAndBlack_Verify()
    {
        // White on start rank (row 6) should have up to two forward moves if empty
        const int whiteSrc = 6 * 8 + 3; // d7
        const Figure whitePike = Figure.LegionaryPike | Figure.IsWhite;

        // Black on start rank (row 1) should have up to two forward moves if empty
        const int blackSrc = 1 * 8 + 4; // e2
        const Figure blackPike = Figure.LegionaryPike | Figure.IsBlack;

        return Verify(new
        {
            White = TestUtils.RunGetActionsScenario(b => { /* ensure path is empty */ },
                whiteSrc,
                whitePike,
                LegionaryPike.GetPossibleActions),
            Black = TestUtils.RunGetActionsScenario(b => { /* ensure path is empty */ },
                blackSrc,
                blackPike,
                LegionaryPike.GetPossibleActions)
        });
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_Verify()
    {
        const Figure pike = Figure.LegionaryPike | Figure.IsWhite;
        return Verify(TestUtils.RunGetActionsScenario(b => { /* setup places pike at edge via src override */ },
            0, // a1
            pike,
            LegionaryPike.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_PromotionToQueen_OnLastRow_WhiteAndBlack_Verify()
    {
        const int whiteSrc = 1 * 8 + 3; // d2 -> d1
        const Figure white = Figure.LegionaryPike | Figure.IsWhite;

        const int blackSrc = 6 * 8 + 4; // e7 -> e8
        const Figure black = Figure.LegionaryPike | Figure.IsBlack;

        return Verify(new
        {
            White = TestUtils.RunGetActionsScenario(b => { /* forward empty */ },
                whiteSrc,
                white,
                LegionaryPike.GetPossibleActions),
            Black = TestUtils.RunGetActionsScenario(b => { /* forward empty */ },
                blackSrc,
                black,
                LegionaryPike.GetPossibleActions)
        });
    }
}
