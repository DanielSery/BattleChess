using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.Figures;

public class FigureBlueprint
{
    // JSON serializable
    public FigureBlueprint()
    {
    }

    public FigureBlueprint(Player player, int figureId, bool isKing)
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