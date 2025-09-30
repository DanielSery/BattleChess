using System.Collections.Generic;
using System.Threading.Tasks;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Engine.Test.Helpers;

namespace CrownsGuard.Engine.Test.Figures;

public class BardTest
{
    [Fact]
    public Task GetPossibleActions_AllEmpty_Verify()
    {
        const int src = 27; // d4
        const Figure bard = Figure.Bard | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(_ => { /* empty around */ },
            src,
            bard,
            Bard.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_KnightEnemiesAndFriendlies_Verify()
    {
        const int src = 27; // d4
        const Figure bard = Figure.Bard | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                foreach (var rel in PositionConstants.KnightPositions)
                {
                    var idx = src.GetWithOffset(rel);
                    if (idx == -1) continue;
                    b[idx] = Figure.Empty; // ensure within board
                }

                var k1 = src.GetWithOffset(PositionConstants.KnightPositions[0]);
                var k2 = src.GetWithOffset(PositionConstants.KnightPositions[1]);
                var k3 = src.GetWithOffset(PositionConstants.KnightPositions[2]);
                var k4 = src.GetWithOffset(PositionConstants.KnightPositions[3]);

                if (k1 != -1) b[k1] = Figure.Peasant | Figure.IsBlack; // enemy -> ConvertUnit
                if (k2 != -1) b[k2] = Figure.Peasant | Figure.IsWhite; // friendly -> PossibleConvertUnit
                if (k3 != -1) b[k3] = Figure.Wall; // non-walkable not enemy -> PossibleConvertUnit
                if (k4 != -1) b[k4] = Figure.Trader | Figure.IsBlack; // enemy -> ConvertUnit
            },
            src,
            bard,
            Bard.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_BlockedBishopMoves_Verify()
    {
        const int src = 27; // d4
        const Figure bard = Figure.Bard | Figure.IsWhite;

        return Verify(TestUtils.RunGetActionsScenario(b =>
            {
                var ul = src.GetWithOffset(PositionConstants.UL);
                var ur = src.GetWithOffset(PositionConstants.UR);
                var dl = src.GetWithOffset(PositionConstants.DL);
                var dr = src.GetWithOffset(PositionConstants.DR);

                if (ul != -1) b[ul] = Figure.Wall; // block move
                if (ur != -1) b[ur] = Figure.LegionarySword | Figure.IsWhite; // friendly blocks move
                if (dl != -1) b[dl] = Figure.LegionarySword | Figure.IsBlack; // enemy blocks move (not walkable)
                if (dr != -1) b[dr] = Figure.Archer | Figure.IsWhite; // friendly blocks move

                // also sprinkle some knight occupants; actions should still be added
                var k1 = src.GetWithOffset(PositionConstants.KnightPositions[4]);
                var k2 = src.GetWithOffset(PositionConstants.KnightPositions[5]);
                if (k1 != -1) b[k1] = Figure.Peasant | Figure.IsBlack; // ConvertUnit
                if (k2 != -1) b[k2] = Figure.Wall; // PossibleConvertUnit
            },
            src,
            bard,
            Bard.GetPossibleActions
        ));
    }

    [Fact]
    public Task GetPossibleActions_EdgeCase_A1_Verify()
    {
        const Figure bard = Figure.Bard | Figure.IsWhite;
        return Verify(TestUtils.RunGetActionsScenario(_ => { /* setup below places bard at edge via src override */ },
            0, // a1
            bard,
            Bard.GetPossibleActions
        ));
    }
}
