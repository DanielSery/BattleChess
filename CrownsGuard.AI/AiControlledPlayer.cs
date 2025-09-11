using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.Game.Players;
using CrownsGuard.Game.Timers;

namespace CrownsGuard.AI;

public class AiControlledPlayer : IAutomaticallyControlledPlayerInfo
{
    public AiControlledPlayer(PlayerColor playerColor)
    {
        PlayerColor = playerColor;
    }

    /// <inheritdoc />
    public string Name => "AI Controlled Player";

    /// <inheritdoc />
    public PlayerColor PlayerColor { get; }

    /// <inheritdoc />
    public IPlayerTimer Timer { get; } = InfinitePlayerTimer.Instance;

    /// <inheritdoc />
    public void SetTimer(IPlayerTimer timer)
    {
    }

    /// <inheritdoc />
    public void StartTurn()
    {
    }

    /// <inheritdoc />
    public void EndTurn(TimeSpan? forcedTurnDuration = null)
    {
    }

    /// <inheritdoc />
    public Task HandleAutomaticTurnAsync()
    {
        return Task.CompletedTask;
    }
}