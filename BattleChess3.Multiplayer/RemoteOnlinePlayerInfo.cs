// Copyright (c) Veeam Software Group GmbH

using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;
using BattleChess3.Game.Timers;

namespace BattleChess3.Multiplayer;

public class RemoteOnlinePlayerInfo : IOnlinePlayerInfo, IAutomaticallyControlledPlayerInfo
{
    private IMultiplayerGameService? _gameService;

    public RemoteOnlinePlayerInfo(Player player, string playerName, string? playerId, int? elo)
    {
        Player = player;
        Name = playerName;
        PlayerId = playerId;
        Elo = elo;
        Timer = InfinitePlayerTimer.Instance;
    }

    public Player Player { get; }
    public IPlayerTimer Timer { get; private set; }
    public string Name { get; }
    public List<Figure> Figures { get; } = [];
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