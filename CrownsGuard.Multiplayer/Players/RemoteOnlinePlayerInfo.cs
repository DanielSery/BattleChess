// Copyright (c) Veeam Software Group GmbH

using CrownsGuard.Core.Figures;
using CrownsGuard.Game.Timers;
using CrownsGuard.Multiplayer.Game;

namespace CrownsGuard.Multiplayer.Players;

public class RemoteOnlinePlayerInfo : IOnlinePlayerInfo, IAutomaticallyControlledPlayerInfo
{
    private IMultiplayerGameService? _gameService;

    public RemoteOnlinePlayerInfo(Core.Players.Player player, string playerName, string? playerId, int? elo)
    {
        Player = player;
        Name = playerName;
        PlayerId = playerId;
        Elo = elo;
        Timer = InfinitePlayerTimer.Instance;
    }

    public Core.Players.Player Player { get; }
    public IPlayerTimer Timer { get; private set; }
    public string Name { get; }
    public List<IFigure> Figures { get; } = [];
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
    public Task HandleAutomaticTurnAsync()
    {
        if (_gameService == null) throw new ArgumentNullException(nameof(_gameService));

        return Task.Run(_gameService.HandleRemotePlayerTurnAsync);
    }

    /// <inheritdoc />
    public void SetTimer(IPlayerTimer timer)
    {
        Timer = timer;
    }

    /// <inheritdoc />
    public void SetGameService(IMultiplayerGameService gameService)
    {
        _gameService = gameService;
    }
}