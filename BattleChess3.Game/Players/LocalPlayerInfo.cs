using System.Diagnostics;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Timers;

namespace BattleChess3.Game.Players;

[DebuggerDisplay("{Name}")]
public class LocalPlayerInfo : ILocalPlayerInfo
{
    public static readonly LocalPlayerInfo Neutral = new(Player.Neutral, "Neutral");

    public LocalPlayerInfo(Player player, string playerName)
    {
        Player = player;
        Name = playerName;
        Timer = InfinitePlayerTimer.Instance;
    }

    public Player Player { get; }
    public IPlayerTimer Timer { get; private set; }
    public string Name { get; }
    public List<Figure> Figures { get; } = [];

    /// <inheritdoc />
    public void SetTimer(IPlayerTimer timer)
    {
        Timer = timer;
    }
}