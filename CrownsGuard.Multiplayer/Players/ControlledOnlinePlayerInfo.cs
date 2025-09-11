using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Game.Players;
using CrownsGuard.Game.Timers;
using CrownsGuard.Multiplayer.Game;

namespace CrownsGuard.Multiplayer.Players;

public class ControlledOnlinePlayerInfo : IOnlinePlayerInfo, IControlledPlayerInfo
{
    public ControlledOnlinePlayerInfo(Core.Players.PlayerColor playerColor, string playerName, string? playerId, int? elo)
    {
        PlayerColor = playerColor;
        Name = playerName;
        Timer = InfinitePlayerTimer.Instance;
        PlayerId = playerId;
        Elo = elo;
    }

    public Core.Players.PlayerColor PlayerColor { get; }
    public IPlayerTimer Timer { get; private set; }
    public string Name { get; }
    public string? PlayerId { get; }
    public int? Elo { get; }

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

    /// <inheritdoc />
    public void SetTimer(IPlayerTimer timer)
    {
        Timer = timer;
    }

    /// <inheritdoc />
    public void SetGameService(IMultiplayerGameService gameService)
    {
    }
}