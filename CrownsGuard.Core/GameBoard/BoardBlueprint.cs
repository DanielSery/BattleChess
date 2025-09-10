using CrownsGuard.Core.Players;
using CrownsGuard.Core.SimulatedBoard;

namespace CrownsGuard.Core.GameBoard;

// JSON serializable
public class BoardBlueprint
{
    public static readonly BoardBlueprint ChessTeam = new()
    {
        Figures = [
            new(Player.White, false, FigureId.LegionarySword),
            new(Player.White, false, FigureId.LegionarySword),
            new(Player.White, false, FigureId.LegionarySword),
            new(Player.White, false, FigureId.LegionarySword),
            new(Player.White, false, FigureId.LegionarySword),
            new(Player.White, false, FigureId.LegionarySword),
            new(Player.White, false, FigureId.LegionarySword),
            new(Player.White, false, FigureId.LegionarySword),
            
            new(Player.White, false, FigureId.MountedKnight),
            new(Player.White, false, FigureId.Whiplash),
            new(Player.White, false, FigureId.CamelRider),
            new(Player.White, true, FigureId.King),
            new(Player.White, false, FigureId.Queen),
            new(Player.White, false, FigureId.CamelRider),
            new(Player.White, false, FigureId.Whiplash),
            new(Player.White, false, FigureId.MountedKnight),
        ],
        StartingPlayer = Player.White
    };
    
    public Player StartingPlayer { get; init; } = Player.White;
    public Figure[] Figures { get; init; } = [];
}