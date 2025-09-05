using System.Diagnostics;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Players;
using CrownsGuard.Game.Timers;

namespace CrownsGuard.Game.Players;

[DebuggerDisplay("{Name}")]
public class ControlledPlayerInfo : IControlledPlayerInfo
{
    public ControlledPlayerInfo(Player player, string playerName)
    {
        Player = player;
        Name = playerName;
        Timer = InfinitePlayerTimer.Instance;
    }

    public Player Player { get; }
    public IPlayerTimer Timer { get; private set; }
    public string Name { get; }
    public List<IFigure> Figures { get; } = [];

    /// <inheritdoc />
    public void SetTimer(IPlayerTimer timer)
    {
        Timer = timer;
    }

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