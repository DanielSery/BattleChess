// Copyright (c) Veeam Software Group GmbH

using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;
using BattleChess3.Game.Timers;

namespace BattleChess3.Multiplayer;

public class LocalOnlinePlayerInfo : IOnlinePlayerInfo, ILocalPlayerInfo
{
    public LocalOnlinePlayerInfo(Player player, string playerName, string? playerId, int? elo)
    {
        Player = player;
        Name = playerName;
        Timer = InfinitePlayerTimer.Instance;
        PlayerId = playerId;
        Elo = elo;
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