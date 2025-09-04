// Copyright (c) Veeam Software Group GmbH

using BattleChess3.Core.Figures;
using BattleChess3.Game.Timers;
using BattleChess3.Multiplayer.Game;

namespace BattleChess3.Multiplayer.Players;

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
        if (_gameService == null) throw new ArgumentNullException(nameof(_gameService));

        Timer.StartTurnTimer();
        _gameService.HandleHisTurnAsync();
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
        _gameService = gameService;
    }
}