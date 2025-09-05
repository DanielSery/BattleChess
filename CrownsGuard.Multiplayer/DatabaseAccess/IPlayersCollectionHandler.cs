// Copyright (c) Veeam Software Group GmbH

using CrownsGuard.Multiplayer.Tables;
using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Multiplayer.DatabaseAccess;

public interface IPlayersCollectionHandler
{
    Task<Result<RegisteredPlayer>> FindPlayerWithId(string? id);

    Task<UpdateResult> UpdatePlayerElo(string? playerId, int newElo);

    Task<Result<RegisteredPlayer>> WaitForPlayerEloUpdate(string? playerId);
}