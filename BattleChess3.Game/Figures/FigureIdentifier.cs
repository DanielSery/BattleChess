using BattleChess3.Game.Players;

namespace BattleChess3.Game.Figures;

public class FigureIdentifier
{
    // JSON serializable
    public FigureIdentifier()
    {
    }

    public FigureIdentifier(int playerId, int uniqueUnitId)
    {
        FigureId = Guid.NewGuid();
        PlayerId = playerId;
        UniqueUnitId = uniqueUnitId;
    }

    public FigureIdentifier(Figure figure)
    {
        FigureId = figure.Id;
        PlayerId = figure.Owner.Id;
        UniqueUnitId = ((IFigureType)figure).UniqueFigureId;
    }

    public FigureIdentifier(int id, IFigureType figureType)
    {
        FigureId = Guid.NewGuid();
        PlayerId = id;
        UniqueUnitId = figureType.UniqueFigureId;
    }

    public Guid FigureId { get; set; }
    public int PlayerId { get; set; } = Player.Neutral.Id;
    public int UniqueUnitId { get; set; } = ((IFigureType)NoneFigureType.Instance).UniqueFigureId;

    public override string ToString()
    {
        return $"{UniqueUnitId}{PlayerId}";
    }
}