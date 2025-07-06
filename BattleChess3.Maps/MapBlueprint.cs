using BattleChess3.Game.Figures;

namespace BattleChess3.Maps;

// JSON serializable
public class MapBlueprint
{
    public static readonly MapBlueprint None = new();

    public static readonly MapBlueprint EmptyTeam = new()
    {
        Figures = Enumerable.Range(0, 16).Select(x => new FigureIdentifier(0, 0, false)).ToArray()
    };

    public static readonly MapBlueprint Empty = new()
    {
        Figures = Enumerable.Range(0, 64).Select(x => new FigureIdentifier(0, 0, false)).ToArray()
    };
    
    public string MapPath { get; init; } = string.Empty;
    public string PreviewPath { get; init; } = string.Empty;
    public Uri? PreviewUri => string.IsNullOrEmpty(PreviewPath) ? null : new Uri(Path.GetFullPath(PreviewPath));
    public int StartingPlayer { get; init; } = 1;
    public FigureIdentifier[] Figures { get; init; } = [];
}