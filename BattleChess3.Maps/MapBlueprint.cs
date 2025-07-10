using BattleChess3.Game.Figures;

namespace BattleChess3.Maps;

// JSON serializable
public class MapBlueprint
{
    public static readonly MapBlueprint EmptyTeam = new()
    {
        Figures = Enumerable.Range(0, 16).Select(x => new FigureIdentifier(0, 0, false)).ToArray()
    };

    public int StartingPlayer { get; init; } = 1;
    public FigureIdentifier[] Figures { get; init; } = [];
}