using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

internal class MultiplayerPlayerService : IMultiplayerPlayerService
{
    private readonly IMongoCollection<Player> _playersCollection;
    private readonly IMultiplayerScheduler _scheduler;

    public MultiplayerPlayerService(IMultiplayerScheduler scheduler)
    {
        _scheduler = scheduler;
        var client = new MongoClient(DbSecrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _playersCollection = database.GetCollection<Player>("Players");
    }

    public Player? LoggedInPlayer { get; private set; }

    /// <inheritdoc />
    public Task<List<PublicPlayerData>> GetLeaderboard(CancellationToken cancellationToken)
    {
        return LoggedInPlayer is null 
            ? GetTopLeaderboard(cancellationToken) 
            : GetUserLeaderboard(LoggedInPlayer.Id, cancellationToken);
    }

    private async Task<List<PublicPlayerData>> GetTopLeaderboard(CancellationToken cancellationToken)
    {
        var bsonCollection = _playersCollection.Database
            .GetCollection<BsonDocument>(_playersCollection.CollectionNamespace.CollectionName);

        var pipeline = new EmptyPipelineDefinition<BsonDocument>()
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$setWindowFields", new BsonDocument
            {
                { "sortBy", new BsonDocument("Elo", -1) },
                { "output", new BsonDocument("Rank", new BsonDocument("$documentNumber", new BsonDocument())) }
            }))
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$limit", 100));
        
        var topDocs = await bsonCollection
            .Aggregate(pipeline, cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken: cancellationToken);
        return topDocs.Select(doc => new PublicPlayerData
        {
            Rank = doc["Rank"].AsInt32,
            Name = doc["Name"].AsString,
            Elo = (short)doc["Elo"].AsInt32
        }).ToList();
    }

    private async Task<List<PublicPlayerData>> GetUserLeaderboard(string playerId, CancellationToken cancellationToken)
    {
        var bsonCollection = _playersCollection.Database
            .GetCollection<BsonDocument>(_playersCollection.CollectionNamespace.CollectionName);
        
        var targetId = ObjectId.Parse(playerId);

        var rankPipeline = new EmptyPipelineDefinition<BsonDocument>()
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$setWindowFields", new BsonDocument
            {
                { "sortBy", new BsonDocument("Elo", -1) },
                { "output", new BsonDocument("Rank", new BsonDocument("$documentNumber", new BsonDocument())) }
            }))
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$match", new BsonDocument("_id", targetId)))
            .AppendStage<BsonDocument, BsonDocument, BsonDocument>(new BsonDocument("$project", new BsonDocument("Rank", 1)));

        var rankDoc = await bsonCollection
            .Aggregate(rankPipeline, cancellationToken: cancellationToken)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (rankDoc == null)
            return [];

        var targetRank = rankDoc["Rank"].AsInt32;int minRank = Math.Max(targetRank - 100, 1);
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

        var leaderboardDocs = await bsonCollection
            .Aggregate(leaderboardPipeline, cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken: cancellationToken);
        return leaderboardDocs.Select(doc => new PublicPlayerData
        {
            Rank = doc["Rank"].AsInt32,
            Name = doc["Name"].AsString,
            Elo = (short)doc["Elo"].AsInt32
        }).ToList();
    }

    public Task<Result<string>> GetUserSaltAsync(string name, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Getting user salt with name: {name}");
                    var filter = Builders<Player>.Filter.Eq("Name", name);
                    var foundPlayers = await _playersCollection.FindAsync(filter, cancellationToken: cancellationToken);
                    var foundPlayer = foundPlayers.FirstOrDefault();
                    if (foundPlayer is null)
                    {
                        Console.WriteLine("Did not find player");
                        return Result.Fail("Wrong username or password");
                    }
                    
                    Console.WriteLine($"Found user with name: {foundPlayer.Name}");
                    return Result.Ok(foundPlayer.PasswordSalt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail<string>(ex.Message);
                }
            });
        }
    }

    public Task<Result> TryLoginAsync(string name, string hash, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Getting users with name: {name}");
                    var filter = Builders<Player>.Filter.And(
                        Builders<Player>.Filter.Eq(g => g.Name, name),
                        Builders<Player>.Filter.Eq(g => g.PasswordHash, hash)
                    );
                    var foundPlayers = await _playersCollection.FindAsync(filter, cancellationToken: cancellationToken);
                    var foundPlayer = foundPlayers.FirstOrDefault();
                    if (foundPlayer is null)
                    {
                        return Result.Fail("Incorrect username or password");
                    }
                    else
                    {
                        Console.WriteLine($"Found player: {foundPlayer.Name}");
                        LoggedInPlayer = foundPlayer;
                        return Result.Ok();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail(ex.Message);
                }
            });
        }
    }

    public Task<Result> TrySignUpAsync(string name, string hash, string salt, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Getting users with name: {name}");
                    var filter = Builders<Player>.Filter.Eq("Name", name);
                    var foundPlayers = await _playersCollection.FindAsync(filter, cancellationToken: cancellationToken);
                    if (await foundPlayers.AnyAsync(cancellationToken: cancellationToken))
                    {
                        Console.WriteLine($"Found user with name: {name}");
                        return Result.Fail($"User with name {name} already exists");
                    }
                    
                    Console.WriteLine($"Creating new player with name: {name}");
                    var player = new Player
                    {
                        Name = name,
                        PasswordHash = hash,
                        PasswordSalt = salt,
                        Elo = 1000,
                        UnlockedFigures = new byte[16]
                    };

                    await _playersCollection.InsertOneAsync(player, cancellationToken: cancellationToken);
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail(ex.Message);
                }
            });
        }
    }
}