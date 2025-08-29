// Copyright (c) Veeam Software Group GmbH

using BattleChess3.Game.Players;

namespace BattleChess3.Multiplayer;

public interface IOnlinePlayerInfo : IPlayerInfo
{
    public string? PlayerId { get; }
    public int? Elo { get; }
}