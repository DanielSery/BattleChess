namespace BattleChess3.Core.Model.Figures;

public class FigureBlueprint
{
    // JSON serializable
    public FigureBlueprint()
    {
    }

    public FigureBlueprint(int id, IFigureType figureType)
    {
        PlayerId = id;
        UnitName = figureType.UnitName;
    }

    public int PlayerId { get; set; } = Player.Neutral.Id;
    public string UnitName { get; set; } = NoneFigure.Instance.UnitName;

    public static implicit operator FigureBlueprint((int id, IFigureType figure) pair)
    {
        return new FigureBlueprint(pair.id, pair.figure);
    }

    public override string ToString()
    {
        return $"{UnitName}{PlayerId}";
    }
}