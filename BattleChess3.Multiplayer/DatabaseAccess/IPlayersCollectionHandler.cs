// Copyright (c) Veeam Software Group GmbH

using System.Linq.Expressions;
using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer.DatabaseAccess;

public interface IPlayersCollectionHandler
{
    Task<Result<RegisteredPlayer>> FindPlayerWithId(string? id);

    Task<UpdateResult> UpdatePlayerWithId<TField>(string? id,
        Expression<Func<RegisteredPlayer, TField>> field,
        TField value);

    Task<Result<RegisteredPlayer>> WaitForPlayerEloUpdate(string? playerId);
}