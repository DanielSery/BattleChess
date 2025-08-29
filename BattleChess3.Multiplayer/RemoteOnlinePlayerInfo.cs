// Copyright (c) Veeam Software Group GmbH

using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.Multiplayer;

public class RemoteOnlinePlayerInfo : IOnlinePlayerInfo, IAutomaticallyControlledPlayerInfo
{
    private readonly IMultiplayerGameService _gameService;

    public RemoteOnlinePlayerInfo(Player player, string playerName, string playerId, int elo, IPlayerTimer timer, IMultiplayerGameService gameService)
    {
        Player = player;
        Name = playerName;
        Timer = timer;
        PlayerId = playerId;
        Elo = elo;
        _gameService = gameService;
    }

    public Player Player { get; }
    public IPlayerTimer Timer { get; }
    public string Name { get; }
    public List<Figure> Figures { get; } = [];
    public string? PlayerId { get; }
    public int? Elo { get; }

    /// <inheritdoc />
    public void StartTurn()
    {
        Timer.StartTurnTimer();
        _gameService.HandleHisTurnAsync();
    }

    /// <inheritdoc />
    public void EndTurn(TimeSpan? forcedTurnDuration = null)
    {
        Timer.EndTurnTimer(forcedTurnDuration);
    }
}