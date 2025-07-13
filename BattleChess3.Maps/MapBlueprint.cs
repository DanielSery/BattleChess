using BattleChess3.Game.Figures;

namespace BattleChess3.Maps;

// JSON serializable
public class MapBlueprint
{
    private static readonly FigureIdentifier[] ChessTeamFigures = [
        new(1, 22, false), new(1, 22, false), new(1, 22, false), new(1, 22, false), new(1, 22, false), new(1, 22, false), new(1, 22, false), new(1, 22, false),
        new(1, 16, false), new(1, 25, false), new(1, 15, false), new(1, 23, true), new(1, 24, false), new(1, 15, false), new(1, 25, false), new(1, 16, false)
    ];

    public static readonly MapBlueprint ChessTeam = new()
    {
        Figures = ChessTeamFigures,
        StartingPlayer = 1
    };
    
    public int StartingPlayer { get; init; } = 1;
    public FigureIdentifier[] Figures { get; init; } = [];
}