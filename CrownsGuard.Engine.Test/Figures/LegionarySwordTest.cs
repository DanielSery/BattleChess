using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class LegionarySwordTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Verify()
    {
        const int src = 27; // d4
        const Figure fig = Figure.LegionarySword | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ => { /* empty around */ },
            src,
            fig,
            LegionarySword.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_StartRow_DoubleStep_White_Verify()
    {
        const int src = (6 * 8) + 3; // d7
        const Figure fig = Figure.LegionarySword | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b => { /* ensure path ahead is empty by default */ },
            src,
            fig,
            LegionarySword.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EnemyLeft_AllyRight_White_Verify()
    {
        const int src = 27; // d4
        const Figure fig = Figure.LegionarySword | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                var ul = src.GetWithOffset(PositionConstants.UL);
                var ur = src.GetWithOffset(PositionConstants.UR);
                if (ul != -1) b[ul] = Figure.Peasant | Figure.IsBlack; // enemy -> MeleeAttack
                if (ur != -1) b[ur] = Figure.Peasant | Figure.IsWhite; // ally -> MeleeDefend
            },
            src,
            fig,
            LegionarySword.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_ForwardBlocked_NoMove_White_Verify()
    {
        const int src = 27; // d4
        const Figure fig = Figure.LegionarySword | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                var forward = src.GetWithOffset(PositionConstants.D); // white forward is D (-Y)
                if (forward != -1) b[forward] = Figure.Wall; // non-walkable blocks move
            },
            src,
            fig,
            LegionarySword.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_PromotionToQueen_OnLastRow_WhiteAndBlack_Verify()
    {
        const int whiteSrc = 1 * 8 + 3; // d2 -> d1 (row 0)
        const Figure white = Figure.LegionarySword | Figure.IsWhite;

        const int blackSrc = 6 * 8 + 4; // e7 -> e8 (row 7)
        const Figure black = Figure.LegionarySword | Figure.IsBlack;

        return Verify(new
        {
            White = TestUtils.RunGetActionsScenario(b => { /* forward square empty, diagonals empty */ },
                whiteSrc,
                white,
                LegionarySword.GetPossibleActions),
            Black = TestUtils.RunGetActionsScenario(b => { /* forward square empty, diagonals empty */ },
                blackSrc,
                black,
                LegionarySword.GetPossibleActions)
        });
    }
}
            