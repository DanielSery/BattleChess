// Copyright (c) Veeam Software Group GmbH

using System.Linq.Expressions;
using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer.DatabaseAccess;

public class PlayersCollectionHandler : IPlayersCollectionHandler
{
    private readonly IMongoCollection<RegisteredPlayer> _playersCollection;

    public PlayersCollectionHandler(IDatabaseClient databaseClient)
    {
        _playersCollection = databaseClient.Players!;
    }
    
    public async Task<Result<RegisteredPlayer>> FindPlayerWithId(string? id)
    {
        try
        {
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, id);
            var foundPlayers = await _playersCollection.FindAsync(playerFilter);
            return await foundPlayers.SingleAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result.Fail(e.Message);
        }
    }

    /// <inheritdoc />
    public async Task<UpdateResult> UpdatePlayerWithId<TField>(string? id, Expression<Func<RegisteredPlayer, TField>> field, TField value)
    {
        try
        {
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, id);
            var update = Builders<RegisteredPlayer>.Update.Set(field, value);
            return await _playersCollection.UpdateOneAsync(playerFilter, update);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return UpdateResult.Unacknowledged.Instance;
        }
    }

    public async Task<Result<RegisteredPlayer>> WaitForPlayerEloUpdate(string? playerId)
    {
        try
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RegisteredPlayer>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Update &&
                                 change.DocumentKey["_id"] == ObjectId.Parse(playerId));

            using var cursor = await _playersCollection.WatchAsync(
                pipeline,
                new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
                cancellationTokenSource.Token
            );

            while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
            {
                foreach (var change in cursor.Current)
                {
                    if (change.FullDocument.Id == playerId)
                    {
                        return change.FullDocument;
                    }
                }
            }

            return Result.Fail("No player update in time.");
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Operation cancelled.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result.Fail(e.Message);
        }
    }
}