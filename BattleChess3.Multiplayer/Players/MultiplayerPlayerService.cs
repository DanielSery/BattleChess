using System.Collections;
using BattleChess3.Core.GameBoard;
using BattleChess3.Core.Players;
using BattleChess3.Multiplayer.Scheduling;
using BattleChess3.Multiplayer.Tables;
using BattleChess3.Multiplayer.Utilities;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer.Players;

internal class MultiplayerPlayerService : IMultiplayerPlayerService
{
    private readonly IMongoCollection<RegisteredPlayer> _playersCollection;
    private readonly IMultiplayerScheduler _scheduler;

    public MultiplayerPlayerService(IMultiplayerScheduler scheduler)
    {
        _scheduler = scheduler;

        var settings = MongoClientSettings.FromConnectionString(Secrets.ConnectionString);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(settings);

        var database = client.GetDatabase("BattleChess");
        _playersCollection = database.GetCollection<RegisteredPlayer>("Players");
    }

    /// <inheritdoc />
    public event EventHandler? LoggedInPlayerChanged;
    
    public RegisteredPlayer? LoggedInPlayer { get; private set; }

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

    public IOnlinePlayerInfo GetCurrentPlayer()
    {
        if (LoggedInPlayer is null)
            return new ControlledOnlinePlayerInfo(Player.White, "Red player", null, null);

        return new ControlledOnlinePlayerInfo(Player.White, LoggedInPlayer.Name, LoggedInPlayer.Id, LoggedInPlayer.Elo);
    }

    public Task<Result<IOnlinePlayerInfo>> GetOpponentPlayerAsync(string playerId, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Getting player with id: {playerId}");
                    var filter = Builders<RegisteredPlayer>.Filter.Eq("Id", playerId);
                    var foundPlayers = await _playersCollection.FindAsync(filter, cancellationToken: cancellationToken);
                    var foundPlayer = await foundPlayers.FirstOrDefaultAsync(cancellationToken);
                    if (foundPlayer is null)
                    {
                        Console.WriteLine("Did not find player");
                        return Result.Fail("Error getting player");
                    }
                    
                    Console.WriteLine($"Found user with id: {foundPlayer.Id}");
                    return Result.Ok<IOnlinePlayerInfo>(new RemoteOnlinePlayerInfo(Player.Black, foundPlayer.Name, playerId, foundPlayer.Elo));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail(ex.Message);
                }
            });
        }
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
                    var filter = Builders<RegisteredPlayer>.Filter.Eq("Name", name);
                    var foundPlayers = await _playersCollection.FindAsync(filter, cancellationToken: cancellationToken);
                    var foundPlayer = await foundPlayers.FirstOrDefaultAsync(cancellationToken);
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
                    var filter = Builders<RegisteredPlayer>.Filter.And(
                        Builders<RegisteredPlayer>.Filter.Eq(g => g.Name, name),
                        Builders<RegisteredPlayer>.Filter.Eq(g => g.PasswordHash, hash)
                    );
                    var foundPlayers = await _playersCollection.FindAsync(filter, cancellationToken: cancellationToken);
                    var foundPlayer = await foundPlayers.FirstOrDefaultAsync(cancellationToken);
                    if (foundPlayer is null)
                    {
                        return Result.Fail("Incorrect username or password");
                    }
                    else
                    {
                        Console.WriteLine($"Found player: {foundPlayer.Name}");
                        LoggedInPlayer = foundPlayer;
                        LoggedInPlayerChanged?.Invoke(this, EventArgs.Empty);
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

    /// <inheritdoc />
    public Task<Result> UpdateCurrentPlayerMapAsync(BoardBlueprint map, CancellationToken cancellationToken)
    {
        if (LoggedInPlayer is null)
            return Task.FromResult(Result.Fail("No logged in player"));
        
        if (!map.IsValid(LoggedInPlayer.UnlockedFigures))
            return Task.FromResult(Result.Fail("Trying to save setup with not unlocked figures"));

        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    var mapData = map.GetByteData();
                    
                    Console.WriteLine($"Updating player setup with name: {LoggedInPlayer.Name}");
                    var filter = Builders<RegisteredPlayer>.Filter.And(
                        Builders<RegisteredPlayer>.Filter.Eq(g => g.Name, LoggedInPlayer.Name),
                        Builders<RegisteredPlayer>.Filter.Eq(g => g.PasswordHash, LoggedInPlayer.PasswordHash)
                    );
                    var update = Builders<RegisteredPlayer>.Update.Set(x => x.Map, mapData);
                    var result = await _playersCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
                    if (!result.IsAcknowledged)
                    {
                        return Result.Fail("Failed to change player setup");
                    }
                    
                    LoggedInPlayer.Map = mapData;
                    LoggedInPlayerChanged?.Invoke(this, EventArgs.Empty);
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

    /// <inheritdoc />
    public Task<Result> UpdateCurrentPlayerUnlockedFigure(int unlockedFigureId, CancellationToken cancellationToken)
    {
        if (LoggedInPlayer is null)
            return Task.FromResult(Result.Fail("No logged in player"));

        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    var unlockedFiguresArray = new BitArray(LoggedInPlayer.UnlockedFigures)
                    {
                        [unlockedFigureId] = true
                    };
                    
                    var newArray = new byte[LoggedInPlayer.UnlockedFigures.Length];
                    unlockedFiguresArray.CopyTo(newArray, 0);

                    Console.WriteLine($"Updating player setup with name: {LoggedInPlayer.Name}");
                    var filter = Builders<RegisteredPlayer>.Filter.And(
                        Builders<RegisteredPlayer>.Filter.Eq(g => g.Name, LoggedInPlayer.Name),
                        Builders<RegisteredPlayer>.Filter.Eq(g => g.PasswordHash, LoggedInPlayer.PasswordHash)
                    );
                    var update = Builders<RegisteredPlayer>.Update.Set(x => x.UnlockedFigures, newArray);
                    var result = await _playersCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
                    if (!result.IsAcknowledged)
                    {
                        return Result.Fail("Failed to change player");
                    }
                    
                    LoggedInPlayer.UnlockedFigures = newArray;
                    LoggedInPlayerChanged?.Invoke(this, EventArgs.Empty);
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

    /// <inheritdoc />
    public Task<Result> TryVerifyEmailAsync(string emailHash, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Getting users with email hash: {emailHash}");
                    var filter = Builders<RegisteredPlayer>.Filter.Eq(g => g.EmailHash, emailHash);
                    var foundPlayers = await _playersCollection.FindAsync(filter, cancellationToken: cancellationToken);
                    var foundPlayer = await foundPlayers.FirstOrDefaultAsync(cancellationToken);
                    if (foundPlayer is not null && string.Equals(foundPlayer.EmailHash, emailHash, StringComparison.Ordinal))
                    {
                        Console.WriteLine($"Found user with email hash: {emailHash}");
                        return Result.Fail("User with given email already exists");
                    }
                    
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

    public Task<Result> TrySignUpAsync(string name, string hash, string salt, string emailHash, BoardBlueprint myMap, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Getting users with name: {name}");
                    var filter = Builders<RegisteredPlayer>.Filter.Or(
                        Builders<RegisteredPlayer>.Filter.Eq(g => g.Name, name),
                        Builders<RegisteredPlayer>.Filter.Eq(g => g.EmailHash, emailHash));
                    var foundPlayers = await _playersCollection.FindAsync(filter, cancellationToken: cancellationToken);
                    var foundPlayer = await foundPlayers.FirstOrDefaultAsync(cancellationToken);
                    if (foundPlayer is not null && string.Equals(foundPlayer.Name, name, StringComparison.Ordinal))
                    {
                        Console.WriteLine($"Found user with name: {name}");
                        return Result.Fail($"User with name {name} already exists");
                    }
                    else if (foundPlayer is not null && string.Equals(foundPlayer.EmailHash, emailHash, StringComparison.Ordinal))
                    {
                        Console.WriteLine($"Found user with email hash: {emailHash}");
                        return Result.Fail($"User with given email already exists");
                    }

                    var mapData = myMap.IsValid(IMultiplayerPlayerService.DefaultUnlockedFigures) 
                        ? myMap.GetByteData() 
                        : BoardBlueprint.ChessTeam.GetByteData();
                    
                    Console.WriteLine($"Creating new player with name: {name}");
                    var player = new RegisteredPlayer
                    {
                        Name = name,
                        PasswordHash = hash,
                        PasswordSalt = salt,
                        EmailHash = emailHash,
                        Elo = 1000,
                        Map = mapData,
                        UnlockedFigures = IMultiplayerPlayerService.DefaultUnlockedFigures,
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