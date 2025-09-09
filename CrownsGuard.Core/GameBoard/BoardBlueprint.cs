using CrownsGuard.Core.Players;
using CrownsGuard.Core.SimulatedBoard;

namespace CrownsGuard.Core.GameBoard;

// JSON serializable
public class BoardBlueprint
{
    public static readonly BoardBlueprint ChessTeam = new()
    {
        Figures = [
            new(Player.White, false, FigureType.LegionarySword),
            new(Player.White, false, FigureType.LegionarySword),
            new(Player.White, false, FigureType.LegionarySword),
            new(Player.White, false, FigureType.LegionarySword),
            new(Player.White, false, FigureType.LegionarySword),
            new(Player.White, false, FigureType.LegionarySword),
            new(Player.White, false, FigureType.LegionarySword),
            new(Player.White, false, FigureType.LegionarySword),
            
            new(Player.White, false, FigureType.MountedKnight),
            new(Player.White, false, FigureType.Whiplash),
            new(Player.White, false, FigureType.CamelRider),
            new(Player.White, true, FigureType.King),
            new(Player.White, false, FigureType.Queen),
            new(Player.White, false, FigureType.CamelRider),
            new(Player.White, false, FigureType.Whiplash),
            new(Player.White, false, FigureType.MountedKnight),
        ],
        StartingPlayer = Player.White
    };
    
    public Player StartingPlayer { get; init; } = Player.White;
    public Figure[] Figures { get; init; } = [];
}