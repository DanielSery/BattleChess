using BattleChess3.Game.Players;

namespace BattleChess3.Game.Figures;

public class FigureIdentifier
{
    // JSON serializable
    public FigureIdentifier()
    {
    }

    public FigureIdentifier(Player player, int figureId, bool isKing)
    {
        Player = player;
        FigureId = figureId;
        IsKing = isKing;
    }

    public FigureIdentifier(Figure figure)
    {
        Player = figure.Owner.Player;
        FigureId = figure.Type.FigureId;
        IsKing = figure.IsKing;
    }

    public FigureIdentifier(Player player, IFigureType figureType, bool isKing)
    {
        Player = player;
        FigureId = figureType.FigureId;
        IsKing = isKing;
    }

    public Player Player { get; set; } = Player.Neutral;
    public int FigureId { get; set; } = NoneFigureType.Instance.FigureId;
    public bool IsKing { get; set; }

    public override string ToString()
    {
        return $"{FigureId} {Player}";
    }
}