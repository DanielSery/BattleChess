using BattleChess3.Game.Board;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.Figures;

public class Figure : IFigureType
{
    public static readonly Figure None = new(Player.Neutral, NoneFigureType.Instance);

    public Figure(Player owner, IFigureType type)
    {
        Owner = owner;
        Type = type;
    }

    public Player Owner { get; }
    public IFigureType Type { get; }
    public Uri ImageUri => Type.ImageUris[Owner.Id];

    public string DisplayName => Type.DisplayName;
    public string Description => Type.Description;
    public string UnitName => Type.UnitName;
    public IDictionary<int, Uri> ImageUris => Type.ImageUris;

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return Type.GetPossibleActions(unitTile, board);
    }

    public override string ToString()
    {
        return $"{Type.DisplayName}:{Owner.Id}";
    }
}