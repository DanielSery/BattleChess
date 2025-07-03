using BattleChess3.Game.Players;

namespace BattleChess3.Game.Figures;

public class FigureIdentifier
{
    // JSON serializable
    public FigureIdentifier()
    {
    }

    public FigureIdentifier(int playerId, int uniqueUnitId, bool isKing)
    {
        FigureId = Guid.NewGuid();
        PlayerId = playerId;
        UniqueUnitId = uniqueUnitId;
        IsKing = isKing;
    }

    public FigureIdentifier(Figure figure)
    {
        FigureId = figure.Id;
        PlayerId = figure.Owner.Id;
        UniqueUnitId = ((IFigureType)figure).UniqueFigureId;
        IsKing = figure.IsKing;
    }

    public FigureIdentifier(int playerId, IFigureType figureType, bool isKing)
    {
        FigureId = Guid.NewGuid();
        PlayerId = playerId;
        UniqueUnitId = figureType.UniqueFigureId;
        IsKing = isKing;
    }

    public Guid FigureId { get; set; }
    public int PlayerId { get; set; } = Player.Neutral.Id;
    public int UniqueUnitId { get; set; } = ((IFigureType)NoneFigureType.Instance).UniqueFigureId;
    public bool IsKing { get; set; }

    public override string ToString()
    {
        return $"{UniqueUnitId} {PlayerId}";
    }
}