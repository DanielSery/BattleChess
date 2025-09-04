// Copyright (c) Veeam Software Group GmbH

using BattleChess3.Game.Players;
using BattleChess3.Multiplayer.Game;

namespace BattleChess3.Multiplayer.Players;

public interface IOnlinePlayerInfo : IPlayerInfo
{
    public string? PlayerId { get; }
    public int? Elo { get; }

    public void SetGameService(IMultiplayerGameService gameService);
}