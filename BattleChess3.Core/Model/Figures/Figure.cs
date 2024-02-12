namespace BattleChess3.Core.Model.Figures;

public class Figure : IFigureType
{
    public static readonly Figure None = new(Player.Neutral, NoneFigure.Instance);

    public Figure(Player owner, IFigureType figureType)
    {
        Owner = owner;
        FigureType = figureType;
    }

    public Player Owner { get; }
    public IFigureType FigureType { get; }
    public Uri ImageUri => FigureType.ImageUris[Owner.Id];

    public string DisplayName => FigureType.DisplayName;
    public string Description => FigureType.Description;
    public string UnitName => FigureType.UnitName;
    public IDictionary<int, Uri> ImageUris => FigureType.ImageUris;

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return FigureType.GetPossibleActions(unitTile, board);
    }

    public override string ToString()
    {
        return $"{FigureType.DisplayName}:{Owner.Id}";
    }
}