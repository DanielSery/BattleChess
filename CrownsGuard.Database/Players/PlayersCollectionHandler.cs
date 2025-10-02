using CrownsGuard.Database.Database;
using CrownsGuard.Database.Errors;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CrownsGuard.Database.Players;

internal class PlayersCollectionHandler : IPlayersCollectionHandler
{
    private readonly IDatabaseClient _client;
    private readonly ILogger<PlayersCollectionHandler> _logger;

    public PlayersCollectionHandler(IDatabaseClient databaseClient, ILogger<PlayersCollectionHandler> logger)
    {
        _client = databaseClient;
        _logger = logger;
    }
    
    public async Task<Result<RegisteredPlayer>> FindPlayerByIdAsync(string id, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Getting player with id: {id}", id);
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, id);
            return await _client.Players.FindSingleResultAsync(playerFilter, cancellationToken: token);
        });
    }

    public async Task<Result<RegisteredPlayer>> FindPlayerByNameAsync(string name, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Getting player with name: {name}", name);
            var filter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Name, name);
            return await _client.Players.FindSingleResultAsync(filter, cancellationToken: token);
        });
    }

    public async Task<Result<bool>> HasPlayerWithEmailHashAsync(string emailHash, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling<bool>(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Getting player with email hash");
            var filter = Builders<RegisteredPlayer>.Filter.Eq(g => g.EmailHash, emailHash);
            var result = await _client.Players.FindSingleResultAsync(filter, cancellationToken: token);
            
            if (result.HasError<NoResultsFoundError>()) return false;
            if (result.IsSuccess || result.HasError<TooManyResultsFoundError>()) return true;
            return result.ToResult();
        });
    }

    public async Task<Result> InsertPlayerAsync(RegisteredPlayer player, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Creating player with id: {playerId}", player.Id);
            await _client.Players.InsertOneAsync(player, cancellationToken: token);
        });
    }

    /// <inheritdoc />
    public async Task<Result> UpdatePlayerEloAsync(string playerId, int newElo, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Updating elo of player with id: {playerId}", playerId);
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, playerId);
            var update = Builders<RegisteredPlayer>.Update.Set(x => x.Elo, newElo);
            var result = await _client.Players.UpdateOneAsync(playerFilter, update, cancellationToken: token);
            return result.ToResult("Failed to update player elo");
        });
    }

    /// <inheritdoc />
    public async Task<Result> UpdatePlayerSetupAsync(string playerId, int[] newMap, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Updating setup of player with id: {playerId}", playerId);
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, playerId);
            var update = Builders<RegisteredPlayer>.Update.Set(x => x.Map, newMap);
            var result = await _client.Players.UpdateOneAsync(playerFilter, update, cancellationToken: token);
            return result.ToResult("Failed to update player setup");
        });
    }

    /// <inheritdoc />
    public async Task<Result> UpdatePlayerUnlockedFiguresAsync(string playerId, byte[] newUnlockedFigures, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Updating unlocked figures of player with id: {playerId}", playerId);
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, playerId);
            var update = Builders<RegisteredPlayer>.Update.Set(x => x.UnlockedFigures, newUnlockedFigures);
            var result = await _client.Players.UpdateOneAsync(playerFilter, update, cancellationToken: token);
            return result.ToResult("Failed to update unlocked figures");
        });
    }

    public async Task<Result<RegisteredPlayer>> WaitForPlayerEloUpdateAsync(string playerId, int initialElo, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Waiting for update of player with id: {playerId}", playerId);
            var streamFilter = Builders<ChangeStreamDocument<RegisteredPlayer>>.Filter.And(
                Builders<ChangeStreamDocument<RegisteredPlayer>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Update),
                Builders<ChangeStreamDocument<RegisteredPlayer>>.Filter.Eq(cs => cs.FullDocument.Id, playerId));
            using var streamCursor = await _client.Players.WatchAsync(streamFilter, cancellationToken: token);
            
            var filter = Builders<RegisteredPlayer>.Filter.And(
                Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, playerId),
                Builders<RegisteredPlayer>.Filter.Ne(g => g.Elo, initialElo));
            var foundResult = await _client.Players.FindFirstResultAsync(filter, cancellationToken: token);

            if (foundResult.IsSuccess) return foundResult;
            return await streamCursor.WaitForUpdateAsync(cancellationToken: token);
        });
    }

    public async Task<Result<List<PublicPlayerData>>> GetTopLeaderboardAsync(CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
        {
            _logger.LogInformation("Getting top leaderboard");
            var bsonCollection = _client.Players.Database
                .GetCollection<BsonDocument>(_client.Players.CollectionNamespace.CollectionName);

            var pipeline = new EmptyPipelineDefinition<BsonDocument>()
                .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$setWindowFields", new BsonDocument
                {
                    { "sortBy", new BsonDocument("Elo", -1) },
                    { "output", new BsonDocument("Rank", new BsonDocument("$documentNumber", new BsonDocument())) }
                }))
                .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$limit", 100));
            
            var topDocs = await bsonCollection
                .Aggregate(pipeline, cancellationToken: token)
                .ToListAsync(cancellationToken: token);

            return topDocs.Select(doc => new PublicPlayerData
            {
                Rank = doc["Rank"].AsInt32,
                Name = doc["Name"].AsString,
                Elo = (short)doc["Elo"].AsInt32
            }).ToList();
        });
    }

    public async Task<Result<List<PublicPlayerData>>> GetUserLeaderboardAsync(string playerId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        return await DatabaseHelper.ExecuteWithErrorHandling(_logger, cancellationToken, timeoutSeconds, async token =>
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
            
            _logger.LogInformation("Retrieving rank of user: {playerId}", playerId);
            var rankDocResult = await bsonCollection
                .Aggregate(rankPipeline, cancellationToken: token)
                .SingleResultAsync(cancellationToken: token);
            
            // ReSharper disable once PossiblyMistakenUseOfCancellationToken
            if (rankDocResult.IsFailed) return await GetTopLeaderboardAsync(cancellationToken, timeoutSeconds);
            _logger.LogInformation("Retrieved rank of user");
            
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
            
            _logger.LogInformation("Retrieving leaderboard for player: {playerId}", playerId);
            var leaderboardDocs = await bsonCollection
                .Aggregate(leaderboardPipeline, cancellationToken: token)
                .ToListAsync(cancellationToken: token);

            var leaderboard = leaderboardDocs.Select(doc => new PublicPlayerData
            {
                Rank = doc["Rank"].AsInt32,
                Name = doc["Name"].AsString,
                Elo = (short)doc["Elo"].AsInt32
            }).ToList();

            _logger.LogInformation("Retrieved leaderboard for player: {playerId}", playerId);
            return leaderboard;
        });
    }
}