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

    public Player Player { get; init; } = Player.Neutral;
    public int FigureId { get; init; } = NoneFigureType.Instance.FigureId;
    public bool IsKing { get; init; }

    public override string ToString()
    {
        return $"{FigureId} {Player}";
    }
}