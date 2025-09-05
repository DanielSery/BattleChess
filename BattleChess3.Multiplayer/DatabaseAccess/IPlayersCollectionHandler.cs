// Copyright (c) Veeam Software Group GmbH

using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer.DatabaseAccess;

public interface IPlayersCollectionHandler
{
    Task<Result<RegisteredPlayer>> FindPlayerWithId(string? id);

    Task<UpdateResult> UpdatePlayerElo(string? playerId, int newElo);

    Task<Result<RegisteredPlayer>> WaitForPlayerEloUpdate(string? playerId);
}