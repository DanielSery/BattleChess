using System.Diagnostics;
using BattleChess3.Game.Figures;

namespace BattleChess3.Game.Players;

[DebuggerDisplay("{Name}")]
public class LocalHumanPlayerInfo : ILocalHumanPlayerInfo
{
    public static readonly LocalHumanPlayerInfo Neutral = new(Player.Neutral, "Neutral", InfinitePlayerTimer.Instance);

    public LocalHumanPlayerInfo(Player player, string playerName, IPlayerTimer timer)
    {
        Player = player;
        Name = playerName;
        Timer = timer;
    }

    public Player Player { get; }
    public IPlayerTimer Timer { get; }
    public string Name { get; }
    public List<Figure> Figures { get; } = [];

    /// <inheritdoc />
    public void StartTurn()
    {
        Timer.StartTurnTimer();
    }

    /// <inheritdoc />
    public void EndTurn(TimeSpan? forcedTurnDuration = null)
    {
        Timer.EndTurnTimer(forcedTurnDuration);
    }
}