// Copyright (c) Veeam Software Group GmbH

using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.Multiplayer;

public class LocalOnlinePlayerInfo : IOnlinePlayerInfo, ILocalHumanPlayerInfo
{
    public LocalOnlinePlayerInfo(Player player, string playerName, string? playerId, int? elo, IPlayerTimer timer)
    {
        Player = player;
        Name = playerName;
        Timer = timer;
        PlayerId = playerId;
        Elo = elo;
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
    }

    /// <inheritdoc />
    public void EndTurn(TimeSpan? forcedTurnDuration = null)
    {
        Timer.EndTurnTimer(forcedTurnDuration);
    }
}