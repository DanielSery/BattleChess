// Copyright (c) Veeam Software Group GmbH

using CrownsGuard.Game.Players;
using CrownsGuard.Multiplayer.Game;

namespace CrownsGuard.Multiplayer.Players;

public interface IOnlinePlayerInfo : IPlayerInfo
{
    public string? PlayerId { get; }
    public int? Elo { get; }

    public void SetGameService(IMultiplayerGameService gameService);
}