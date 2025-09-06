// Copyright (c) Veeam Software Group GmbH

using CrownsGuard.Database.Database;
using CrownsGuard.Database.Utilities;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CrownsGuard.Database.Players;

internal class PlayersCollectionHandler : IPlayersCollectionHandler
{
    private readonly IDatabaseClient _client;

    public PlayersCollectionHandler(IDatabaseClient databaseClient)
    {
        _client = databaseClient;
    }
    
    public async Task<Result<RegisteredPlayer>> FindByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Getting player with id: {id}");
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, id);
            var foundPlayers = await _client.Players.FindAsync(playerFilter, cancellationToken: cancellationToken);
            var foundPlayerResult = await foundPlayers.SingleResultAsync(cancellationToken: cancellationToken);
            if (foundPlayerResult.IsSuccess)
            {
                Console.WriteLine($"Found player {foundPlayerResult.Value.Name} with id: {foundPlayerResult.Value.Id}");
            }
            return foundPlayerResult;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to get player with id: {id}, exception: {e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<RegisteredPlayer>> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Getting player with name: {name}");
            var filter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Name, name);
            var foundPlayers = await _client.Players.FindAsync(filter, cancellationToken: cancellationToken);
            var foundPlayerResult = await foundPlayers.SingleResultAsync(cancellationToken: cancellationToken);
            if (foundPlayerResult.IsSuccess)
            {
                Console.WriteLine($"Found player {foundPlayerResult.Value.Name} with id: {foundPlayerResult.Value.Id}");
            }
            return foundPlayerResult;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to find player with name: {name}, exception: {e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<RegisteredPlayer>> FindByEmailHashAsync(string emailHash, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Getting player with email hash");
            var filter = Builders<RegisteredPlayer>.Filter.Eq(g => g.EmailHash, emailHash);
            var foundPlayers = await _client.Players.FindAsync(filter, cancellationToken: cancellationToken);
            var foundPlayerResult = await foundPlayers.SingleResultAsync(cancellationToken: cancellationToken);
            if (foundPlayerResult.IsSuccess)
            {
                Console.WriteLine($"Found player {foundPlayerResult.Value.Name} with id: {foundPlayerResult.Value.Id}");
            }
            return foundPlayerResult;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to find player with email hash exception: {e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> InsertAsync(RegisteredPlayer player, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Creating player with id: {player.Id}");
            await _client.Players.InsertOneAsync(player, cancellationToken: cancellationToken);
            Console.WriteLine($"Created player with id: {player.Id}");
            return Result.Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to create player with id: {player.Id}, exception: {e}");
            return Result.Fail(e.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result> UpdateEloAsync(string playerId, int newElo, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Updating elo of player with id: {playerId}");
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, playerId);
            var update = Builders<RegisteredPlayer>.Update.Set(x => x.Elo, newElo);
            var updateResult = await _client.Players.UpdateOneAsync(playerFilter, update, cancellationToken: cancellationToken);
            Console.WriteLine($"Updating elo of player with id: {playerId} was {updateResult.IsAcknowledged}");
            return updateResult.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to update player elo");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to update elo of player with id: {playerId}, exception: {e}");
            return Result.Fail(e.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result> UpdateSetupAsync(string playerId, byte[] newMap, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Updating setup of player with id: {playerId}");
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, playerId);
            var update = Builders<RegisteredPlayer>.Update.Set(x => x.Map, newMap);
            var updateResult = await _client.Players.UpdateOneAsync(playerFilter, update, cancellationToken: cancellationToken);
            Console.WriteLine($"Updating setup of player with id: {playerId} was {updateResult.IsAcknowledged}");
            return updateResult.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to update player setup");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to update setup of player with id: {playerId}, exception: {e}");
            return Result.Fail(e.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result> UpdateUnlockedFiguresAsync(string playerId, byte[] newUnlockedFigures, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Updating unlocked figures of player with id: {playerId}");
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, playerId);
            var update = Builders<RegisteredPlayer>.Update.Set(x => x.UnlockedFigures, newUnlockedFigures);
            var updateResult = await _client.Players.UpdateOneAsync(playerFilter, update, cancellationToken: cancellationToken);
            Console.WriteLine($"Updating unlocked figures of player with id: {playerId} was {updateResult.IsAcknowledged}");
            return updateResult.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to update unlocked figures");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to update unlocked figures of player with id: {playerId}, exception: {e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<RegisteredPlayer>> WaitForEloUpdateAsync(string playerId, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Waiting for update of player with id: {playerId}");
            using var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RegisteredPlayer>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Update &&
                                 change.DocumentKey["_id"] == ObjectId.Parse(playerId));

            using var cursor = await _client.Players.WatchAsync(
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
                        Console.WriteLine($"Found update of player with id: {change.FullDocument.Id}");
                        return change.FullDocument;
                    }
                }
            }

            Console.WriteLine($"No update of player with id: {playerId} in time");
            return Result.Fail("No player update in time.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Cancelling waiting for update of player with id: {playerId}");
            return Result.Fail("Operation cancelled.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed waiting for update of player with id: {playerId}, exception: {e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<List<PublicPlayerData>>> GetTopLeaderboardAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Getting top leaderboard");
        var bsonCollection = _client.Players.Database
            .GetCollection<BsonDocument>(_client.Players.CollectionNamespace.CollectionName);

        var pipeline = new EmptyPipelineDefinition<BsonDocument>()
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$setWindowFields", new BsonDocument
            {
                { "sortBy", new BsonDocument("Elo", -1) },
                { "output", new BsonDocument("Rank", new BsonDocument("$documentNumber", new BsonDocument())) }
            }))
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$limit", 100));

        try
        {
            var topDocs = await bsonCollection
                .Aggregate(pipeline, cancellationToken: cancellationToken)
                .ToListAsync(cancellationToken: cancellationToken);

            var leaderboard = topDocs.Select(doc => new PublicPlayerData
            {
                Rank = doc["Rank"].AsInt32,
                Name = doc["Name"].AsString,
                Elo = (short)doc["Elo"].AsInt32
            }).ToList();

            Console.WriteLine("Retrieved top leaderboard");
            return leaderboard;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed retrieving top leaderboard, exception: {e}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<List<PublicPlayerData>>> GetUserLeaderboardAsync(string playerId, CancellationToken cancellationToken)
    {
        var bsonCollection = _client.Players.Database
            .GetCollection<BsonDocument>(_client.Players.CollectionNamespace.CollectionName);

        var targetId = ObjectId.Parse(playerId);

        var rankPipeline = new EmptyPipelineDefinition<BsonDocument>()
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$setWindowFields", new BsonDocument
            {
                { "sortBy", new BsonDocument("Elo", -1) },
                { "output", new BsonDocument("Rank", new BsonDocument("$documentNumber", new BsonDocument())) }
            }))
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$match", new BsonDocument("_id", targetId)))
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$project", new BsonDocument("Rank", 1)));

        Result<BsonDocument> rankDocResult;
        try
        {
            Console.WriteLine($"Retrieving rank of user: {playerId}");
            rankDocResult = await bsonCollection
                .Aggregate(rankPipeline, cancellationToken: cancellationToken)
                .SingleResultAsync(cancellationToken: cancellationToken);
            if (rankDocResult.IsFailed) return Result.Fail("Failed to retrieve user's rank");
            Console.WriteLine("Retrieved rank of user");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed retrieving rank of user: {playerId}, exception: {e}");
            return Result.Fail(e.Message);
        }

        var targetRank = rankDocResult.Value["Rank"].AsInt32;
        var minRank = Math.Max(targetRank - 100, 1);
        var maxRank = targetRank + 100;

        var leaderboardPipeline = new EmptyPipelineDefinition<BsonDocument>()
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$setWindowFields", new BsonDocument
            {
                { "sortBy", new BsonDocument("Elo", -1) },
                { "output", new BsonDocument("Rank", new BsonDocument("$documentNumber", new BsonDocument())) }
            }))
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$match", new BsonDocument
            {
                { "Rank", new BsonDocument("$gte", minRank).Add("$lte", maxRank) }
            }));

        try
        {
            Console.WriteLine($"Retrieving leaderboard for player: {playerId}");
            var leaderboardDocs = await bsonCollection
                .Aggregate(leaderboardPipeline, cancellationToken: cancellationToken)
                .ToListAsync(cancellationToken: cancellationToken);

            var leaderboard = leaderboardDocs.Select(doc => new PublicPlayerData
            {
                Rank = doc["Rank"].AsInt32,
                Name = doc["Name"].AsString,
                Elo = (short)doc["Elo"].AsInt32
            }).ToList();

            Console.WriteLine($"Retrieved leaderboard for player: {playerId}");
            return leaderboard;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed retrieving leaderboard for player: {playerId}, exception: {e}");
            return Result.Fail(e.Message);
        }
    }
}