using System.Diagnostics.CodeAnalysis;
using CrownsGuard.Database.Database;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CrownsGuard.Database.Players;

[SuppressMessage("ReSharper", "PossiblyMistakenUseOfCancellationToken")]
internal class PlayersCollectionHandler : IPlayersCollectionHandler
{
    private readonly IDatabaseClient _client;
    private readonly ILogger<PlayersCollectionHandler> _logger;

    public PlayersCollectionHandler(IDatabaseClient databaseClient, ILogger<PlayersCollectionHandler> logger)
    {
        _client = databaseClient;
        _logger = logger;
    }
    
    public async Task<Result<RegisteredPlayer>> FindByIdAsync(string id, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Getting player with id: {id}", id);
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, id);
            return await _client.Players.FindSingleResultAsync(playerFilter, cancellationToken: linkedSource.Token);
            
        }, nameof(FindByIdAsync), _logger, cancellationToken);
    }

    public async Task<Result<RegisteredPlayer>> FindByNameAsync(string name, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Getting player with name: {name}", name);
            var filter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Name, name);
            return await _client.Players.FindSingleResultAsync(filter, cancellationToken: linkedSource.Token);
            
        }, nameof(FindByNameAsync), _logger, cancellationToken);
    }

    public async Task<Result<RegisteredPlayer>> FindByEmailHashAsync(string emailHash, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Getting player with email hash");
            var filter = Builders<RegisteredPlayer>.Filter.Eq(g => g.EmailHash, emailHash);
            return await _client.Players.FindSingleResultAsync(filter, cancellationToken: linkedSource.Token);
            
        }, nameof(FindByEmailHashAsync), _logger, cancellationToken);
    }

    public async Task<Result> InsertAsync(RegisteredPlayer player, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Creating player with id: {playerId}", player.Id);
            await _client.Players.InsertOneAsync(player, cancellationToken: linkedSource.Token);
            
        }, nameof(InsertAsync), _logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> UpdateEloAsync(string playerId, int newElo, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Updating elo of player with id: {playerId}", playerId);
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, playerId);
            var update = Builders<RegisteredPlayer>.Update.Set(x => x.Elo, newElo);
            var updateResult = await _client.Players.UpdateOneAsync(playerFilter, update, cancellationToken: linkedSource.Token);
            return updateResult.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to update player elo");
            
        }, nameof(UpdateEloAsync), _logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> UpdateSetupAsync(string playerId, int[] newMap, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Updating setup of player with id: {playerId}", playerId);
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, playerId);
            var update = Builders<RegisteredPlayer>.Update.Set(x => x.Map, newMap);
            var updateResult = await _client.Players.UpdateOneAsync(playerFilter, update, cancellationToken: linkedSource.Token);
            return updateResult.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to update player setup");
            
        }, nameof(UpdateSetupAsync), _logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> UpdateUnlockedFiguresAsync(string playerId, byte[] newUnlockedFigures, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Updating unlocked figures of player with id: {playerId}", playerId);
            var playerFilter = Builders<RegisteredPlayer>.Filter.Eq(g => g.Id, playerId);
            var update = Builders<RegisteredPlayer>.Update.Set(x => x.UnlockedFigures, newUnlockedFigures);
            var updateResult = await _client.Players.UpdateOneAsync(playerFilter, update, cancellationToken: linkedSource.Token);
            return updateResult.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to update unlocked figures");
            
        }, nameof(UpdateUnlockedFiguresAsync), _logger, cancellationToken);
    }

    public async Task<Result<RegisteredPlayer>> WaitForEloUpdateAsync(string playerId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Waiting for update of player with id: {playerId}", playerId);
            var streamFilter = Builders<ChangeStreamDocument<RegisteredPlayer>>.Filter.And(
                Builders<ChangeStreamDocument<RegisteredPlayer>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Update),
                Builders<ChangeStreamDocument<RegisteredPlayer>>.Filter.Eq(cs => cs.FullDocument.Id, playerId));
            using var streamCursor = await _client.Players.CreateChangeStreamCursorAsync(streamFilter, cancellationToken: linkedSource.Token);
            return await streamCursor.WaitForAddAsync(cancellationToken: linkedSource.Token);
            
        }, nameof(WaitForEloUpdateAsync), _logger, cancellationToken);
    }

    public async Task<Result<List<PublicPlayerData>>> GetTopLeaderboardAsync(CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
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
                .Aggregate(pipeline, cancellationToken: linkedSource.Token)
                .ToListAsync(cancellationToken: linkedSource.Token);

            return topDocs.Select(doc => new PublicPlayerData
            {
                Rank = doc["Rank"].AsInt32,
                Name = doc["Name"].AsString,
                Elo = (short)doc["Elo"].AsInt32
            }).ToList();
            
            
        }, nameof(GetTopLeaderboardAsync), _logger, cancellationToken);
    }

    public async Task<Result<List<PublicPlayerData>>> GetUserLeaderboardAsync(string playerId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
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
                .Aggregate(rankPipeline, cancellationToken: linkedSource.Token)
                .SingleResultAsync(cancellationToken: linkedSource.Token);
            if (rankDocResult.IsFailed) return await GetTopLeaderboardAsync(linkedSource.Token, timeoutSeconds);
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
                .Aggregate(leaderboardPipeline, cancellationToken: linkedSource.Token)
                .ToListAsync(cancellationToken: linkedSource.Token);

            var leaderboard = leaderboardDocs.Select(doc => new PublicPlayerData
            {
                Rank = doc["Rank"].AsInt32,
                Name = doc["Name"].AsString,
                Elo = (short)doc["Elo"].AsInt32
            }).ToList();

            _logger.LogInformation("Retrieved leaderboard for player: {playerId}", playerId);
            return leaderboard;
            
        }, nameof(GetUserLeaderboardAsync), _logger, cancellationToken);
    }
}