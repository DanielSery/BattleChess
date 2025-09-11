using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.GameBoard;

// JSON serializable
public class BoardBlueprint
{
    public static readonly BoardBlueprint ChessTeam = new()
    {
        Figures = [
            new(PlayerColor.White, false, FigureId.LegionarySword),
            new(PlayerColor.White, false, FigureId.LegionarySword),
            new(PlayerColor.White, false, FigureId.LegionarySword),
            new(PlayerColor.White, false, FigureId.LegionarySword),
            new(PlayerColor.White, false, FigureId.LegionarySword),
            new(PlayerColor.White, false, FigureId.LegionarySword),
            new(PlayerColor.White, false, FigureId.LegionarySword),
            new(PlayerColor.White, false, FigureId.LegionarySword),
            
            new(PlayerColor.White, false, FigureId.MountedKnight),
            new(PlayerColor.White, false, FigureId.Whiplash),
            new(PlayerColor.White, false, FigureId.CamelRider),
            new(PlayerColor.White, false, FigureId.Queen),
            new(PlayerColor.White, true, FigureId.King),
            new(PlayerColor.White, false, FigureId.CamelRider),
            new(PlayerColor.White, false, FigureId.Whiplash),
            new(PlayerColor.White, false, FigureId.MountedKnight),
        ],
        StartingPlayerColor = PlayerColor.White
    };
    
    public PlayerColor StartingPlayerColor { get; init; } = PlayerColor.White;
    public Figure[] Figures { get; init; } = [];
}