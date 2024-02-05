namespace BattleChess3.Core.Model.Figures;

public class FigureBlueprint
{
    public int PlayerId { get; set; } = Player.Neutral.Id;
    public string UnitName { get; set; } = NoneFigure.Instance.UnitName;

    // JSON serializable
    public FigureBlueprint()
    {
    }
    
    public FigureBlueprint(int id, IFigureType figureType)
    {
        PlayerId = id;
        UnitName = figureType.UnitName;
    }

    public static implicit operator FigureBlueprint((int id, IFigureType figure) pair)
        => new(pair.id, pair.figure);

    public override string ToString() => $"{UnitName}{PlayerId}";
}
