using CrownsGuard.Core.Figures;
using CrownsGuard.Game.Players;

namespace CrownsGuard.Maps.BoardBlueprints;

// JSON serializable
public class BoardBlueprint
{
    public static readonly BoardBlueprint ChessTeam = new()
    {
        Figures = [
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
        ],
        StartingPlayerColor = PlayerColor.White
    };
    
    public PlayerColor StartingPlayerColor { get; init; } = PlayerColor.White;
    public Figure[] Figures { get; init; } = [];
}