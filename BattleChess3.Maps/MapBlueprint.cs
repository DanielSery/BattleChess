using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.Maps;

// JSON serializable
public class MapBlueprint
{
    private static readonly FigureIdentifier[] ChessTeamFigures = [
        new(Player.White, 22, false), new(Player.White,22, false), new(Player.White,22, false), new(Player.White,22, false), new(Player.White,22, false), new(Player.White,22, false), new(Player.White,22, false), new(Player.White,22, false),
        new(Player.White,16, false), new(Player.White,25, false), new(Player.White,15, false), new(Player.White,23, true), new(Player.White,24, false), new(Player.White,15, false), new(Player.White,25, false), new(Player.White,16, false)
    ];

    public static readonly MapBlueprint ChessTeam = new()
    {
        Figures = ChessTeamFigures,
        StartingPlayer = Player.White
    };
    
    public Player StartingPlayer { get; init; } = Player.White;
    public FigureIdentifier[] Figures { get; init; } = [];
}