using BattleChess3.Game.Players;

namespace BattleChess3.Game.Figures;

public class FigureIdentifier
{
    // JSON serializable
    public FigureIdentifier()
    {
    }

    public FigureIdentifier(int playerId, int figureId, bool isKing)
    {
        PlayerId = playerId;
        FigureId = figureId;
        IsKing = isKing;
    }

    public FigureIdentifier(Figure figure)
    {
        PlayerId = figure.Owner.Id;
        FigureId = ((IFigureType)figure).FigureId;
        IsKing = figure.IsKing;
    }

    public FigureIdentifier(int playerId, IFigureType figureType, bool isKing)
    {
        PlayerId = playerId;
        FigureId = figureType.FigureId;
        IsKing = isKing;
    }

    public int PlayerId { get; set; } = Player.Neutral.Id;
    public int FigureId { get; set; } = ((IFigureType)NoneFigureType.Instance).FigureId;
    public bool IsKing { get; set; }

    public override string ToString()
    {
        return $"{FigureId} {PlayerId}";
    }
}