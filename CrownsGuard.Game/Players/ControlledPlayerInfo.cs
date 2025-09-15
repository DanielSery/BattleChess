using System.Diagnostics;
using CrownsGuard.Core.Players;
using CrownsGuard.Game.Timers;

namespace CrownsGuard.Game.Players;

[DebuggerDisplay("{Name}")]
public class ControlledPlayerInfo : IControlledPlayerInfo
{
    public ControlledPlayerInfo(PlayerColor playerColor, string playerName)
    {
        PlayerColor = playerColor;
        Name = playerName;
        Timer = InfinitePlayerTimer.Instance;
    }

    public PlayerColor PlayerColor { get; }
    public IPlayerTimer Timer { get; private set; }
    public string Name { get; }

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