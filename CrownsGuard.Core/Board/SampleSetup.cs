using CrownsGuard.Core.Figures;

namespace CrownsGuard.Core.Board;

public static class SampleSetup
{
    public static readonly Figure[] ChessSetup = [
        Figure.LegionarySword | Figure.IsWhite,
        Figure.LegionarySword | Figure.IsWhite,
        Figure.LegionarySword | Figure.IsWhite,
        Figure.LegionarySword | Figure.IsWhite,
        Figure.LegionarySword | Figure.IsWhite,
        Figure.LegionarySword | Figure.IsWhite,
        Figure.LegionarySword | Figure.IsWhite,
        Figure.LegionarySword | Figure.IsWhite,
            
        Figure.MountedKnight | Figure.IsWhite,
        Figure.Whiplash | Figure.IsWhite,
        Figure.CamelRider | Figure.IsWhite,
        Figure.Queen | Figure.IsWhite,
        Figure.King | Figure.IsWhite | Figure.IsKing,
        Figure.CamelRider | Figure.IsWhite,
        Figure.Whiplash | Figure.IsWhite,
        Figure.MountedKnight | Figure.IsWhite,
    ];
}