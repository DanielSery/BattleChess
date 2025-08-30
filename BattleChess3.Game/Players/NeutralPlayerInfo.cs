using BattleChess3.Game.Figures;
using BattleChess3.Game.Timers;

namespace BattleChess3.Game.Players;

public class NeutralPlayerInfo : IPlayerInfo
{
    public static readonly NeutralPlayerInfo Instance = new();

    private NeutralPlayerInfo() {}

    public Player Player => Player.Neutral;
    public IPlayerTimer Timer => InfinitePlayerTimer.Instance;
    public string Name => "Neutral";
    public List<IFigure> Figures { get; } = [];

    /// <inheritdoc />
    public void SetTimer(IPlayerTimer timer)
    {
        throw new NotSupportedException("Neutral player timer is not supported");
    }
}