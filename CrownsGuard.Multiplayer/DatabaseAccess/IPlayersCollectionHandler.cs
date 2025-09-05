// Copyright (c) Veeam Software Group GmbH

using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Tables;
using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Multiplayer.DatabaseAccess;

public interface IPlayersCollectionHandler
{
    Task<Result<RegisteredPlayer>> FindPlayerById(string id, CancellationToken cancellationToken);

    Task<Result<RegisteredPlayer>> FindPlayerByName(string name, CancellationToken cancellationToken);

    Task<Result<RegisteredPlayer>> FindPlayerByEmailHash(string emailHash, CancellationToken cancellationToken);

    Task<Result<RegisteredPlayer>> FindPlayerByNameAndHash(string name, string passwordHash, CancellationToken cancellationToken);

    Task<Result> InsertPlayer(RegisteredPlayer player, CancellationToken cancellationToken);
    Task<Result> UpdatePlayerElo(string playerId, int newElo, CancellationToken cancellationToken);
    Task<Result> UpdatePlayerSetup(string playerId, byte[] newMap, CancellationToken cancellationToken);
    Task<Result> UpdatePlayerUnlockedFigures(string playerId, byte[] newUnlockedFigures, CancellationToken cancellationToken);

    Task<Result<RegisteredPlayer>> WaitForPlayerEloUpdate(string playerId, CancellationToken cancellationToken);

    Task<Result<List<PublicPlayerData>>> GetTopLeaderboard(CancellationToken cancellationToken);

    Task<Result<List<PublicPlayerData>>> GetUserLeaderboard(string playerId, CancellationToken cancellationToken);
}