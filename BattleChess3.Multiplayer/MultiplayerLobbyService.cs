using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using BattleChess3.Maps;
using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

internal class MultiplayerLobbyService : IMultiplayerLobbyService
{
    private readonly int _version;
    
    private readonly IMultiplayerScheduler _scheduler;
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    
    private readonly IMongoCollection<GameLobby> _gameLobbyCollection;
    private readonly IMongoCollection<GameLobbyJoin> _gameJoinsCollection;

    public MultiplayerLobbyService(
        IMultiplayerScheduler scheduler,
        IMultiplayerPlayerService multiplayerPlayerService)
    {
        _scheduler = scheduler;
        _multiplayerPlayerService = multiplayerPlayerService;
        
        var client = new MongoClient(Secrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _gameLobbyCollection = database.GetCollection<GameLobby>("GameLobbies");
        _gameJoinsCollection = database.GetCollection<GameLobbyJoin>("GameLobbyJoins");

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        _version = version is not null 
            ? ((byte)version.Major) << 16 | (byte)version.Minor << 8 | (byte)version.Revision
            : -1;
    }

    public Task<List<PublicLobbyData>> GetPublicLobbiesAsync(CancellationToken cancellationToken)
    { 
        return _gameLobbyCollection.Aggregate()
            .Match(l => l.Version == _version && string.IsNullOrEmpty(l.JoinedId))
            .Project(doc => new PublicLobbyData
            {
                Id = doc.Id,
                LobbyName = doc.LobbyName,
                Elo = doc.Elo,
                Locked = (doc.PasswordHash.Length > 0) ? "True" : "False",
            })
            .ToListAsync(cancellationToken: cancellationToken);
    }
    
    public Task WatchLobbiesAsync(
        Action<PublicLobbyData> onLobbyAdded,
        Action<PublicLobbyData> onLobbyChanged,
        Action<string> onLobbyRemoved,  // pass removed lobby Id as string
        CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameLobby>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Insert ||
                                 change.OperationType == ChangeStreamOperationType.Replace ||
                                 change.OperationType == ChangeStreamOperationType.Update ||
                                 change.OperationType == ChangeStreamOperationType.Delete);

            using var cursor = _gameLobbyCollection.Watch(pipeline, cancellationToken: cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (!cursor.MoveNext(cancellationToken)) 
                    continue;
                
                foreach (var change in cursor.Current)
                {
                    switch (change.OperationType)
                    {
                        case ChangeStreamOperationType.Insert:
                            if (change.FullDocument != null)
                            {
                                onLobbyAdded?.Invoke(new PublicLobbyData
                                {
                                    Id = change.FullDocument.Id,
                                    LobbyName = change.FullDocument.LobbyName,
                                    Elo = change.FullDocument.Elo,
                                    JoinedId = change.FullDocument.JoinedId,
                                    Locked = (change.FullDocument.PasswordHash.Length > 0) ? "True" : "False",
                                });
                            }
                            break;

                        case ChangeStreamOperationType.Replace:
                        case ChangeStreamOperationType.Update:
                            PublicLobbyData? lobby;
                            if (change.FullDocument != null)
                            {
                                lobby = new PublicLobbyData
                                {
                                    Id = change.FullDocument.Id,
                                    LobbyName = change.FullDocument.LobbyName,
                                    Elo = change.FullDocument.Elo,
                                    JoinedId = change.FullDocument.JoinedId,
                                    Locked = (change.FullDocument.PasswordHash.Length > 0) ? "True" : "False",
                                };
                            }
                            else
                            {
                                var id = change.DocumentKey["_id"].AsObjectId.ToString();
                                var foundLobby = _gameLobbyCollection.Find(l => l.Id == id).FirstOrDefault(cancellationToken);
                                if (foundLobby is null)
                                {
                                    lobby = null;
                                }
                                else
                                {
                                    lobby = new PublicLobbyData
                                    {
                                        Id = id,
                                        LobbyName = foundLobby.LobbyName,
                                        Elo = foundLobby.Elo,
                                        JoinedId = foundLobby.JoinedId,
                                        Locked = (foundLobby.PasswordHash.Length > 0) ? "True" : "False",
                                    };
                                }
                            }

                            if (lobby is not null)
                                onLobbyChanged?.Invoke(lobby);
                            break;

                        case ChangeStreamOperationType.Delete:
                            var removedId = change.DocumentKey["_id"].AsObjectId.ToString();
                            onLobbyRemoved?.Invoke(removedId);
                            break;
                    }
                }
            }
        }, cancellationToken);
    }

    public Task<Result<GameLobby>> CreateLobbyAsync(
        string lobbyName,
        string password,
        MapBlueprint myMap,
        CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            var random = new Random();
            var isHostStarting = random.Next(0, 1) == 1;
            
            return _scheduler.QueueTask(async () =>
            {
                var myMapData = GetMapData(myMap);
                try
                {
                    Console.WriteLine($"Searching for lobby with name: {lobbyName}");
                    var filter = Builders<GameLobby>.Filter.And(
                        Builders<GameLobby>.Filter.Eq(g => g.LobbyName, lobbyName),
                        Builders<GameLobby>.Filter.Eq(g => g.Version, _version)
                    );
                    var foundGames = await _gameLobbyCollection.FindAsync(filter, cancellationToken: cancellationToken);
                    if (await foundGames.AnyAsync(cancellationToken: cancellationToken))
                    {
                        return Result.Fail<GameLobby>("Lobby already exists");
                    }
                    Console.WriteLine($"Found lobby with name: {lobbyName}");
                    
                    var currentPlayer = _multiplayerPlayerService.LoggedInPlayer;
                    var salt = GetSalt();
                    var hash = GetHash(password, salt);
                    
                    Console.WriteLine("Creating game request");
                    var game = new GameLobby
                    {
                        LobbyName = lobbyName,
                        PasswordHash = hash,
                        PasswordSalt = salt,
                        Map = myMapData,
                        PlayerId = currentPlayer?.Id ?? null,
                        Elo = currentPlayer?.Elo ?? null,
                        Version = _version,
                        IsHostStarting = isHostStarting,
                    };
                    await _gameLobbyCollection.InsertOneAsync(game, cancellationToken: cancellationToken);
                    Console.WriteLine($"Created game request: {game.Id}");
                    return Result.Ok(game);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail("Failed to create lobby");
                }
            });
        }
    }

    public Task<Result<GameLobbyJoin>> WaitForLobbyPlayerAsync(GameLobby lobby, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Waiting for join request for game: {lobby.Id}");
                    var joinRequest = await WaitForGameJoinAsync(lobby.Id, cancellationToken);
                    if (joinRequest is null)
                    {
                        await DeleteGameAsync(lobby.Id);
                        return Result.Fail("Timeout waiting for joining player");
                    }

                    Console.WriteLine($"Found join request: {joinRequest.Id}");

                    Console.WriteLine("Confirming game join");
                    var filter = Builders<GameLobby>.Filter.Eq(l => l.Id, lobby.Id);
                    var update = Builders<GameLobby>.Update.Set(x => x.JoinedId, joinRequest.Id);
                    var result = await _gameLobbyCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
                    if (!result.IsAcknowledged)
                    {
                        await DeleteGameAsync(lobby.Id);
                        return Result.Fail("Failed to update game confirmation");
                    }

                    Console.WriteLine($"Confirmed game join for request: {joinRequest.Id}");
                    return Result.Ok(joinRequest);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail("Failed to wait for joining player");
                }
                finally
                {
                    if (lobby.JoinedId is null)
                    {
                        await DeleteGameAsync(lobby.Id);
                    }
                }
            });
        }
    }

    public Task<Result<GameLobby>> JoinLobbyAsync(
        string lobbyName,
        string password,
        MapBlueprint myMap,
        CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {         
                    Console.WriteLine($"Searching for lobby with name: {lobbyName}");
                    var filter = Builders<GameLobby>.Filter.And(
                        Builders<GameLobby>.Filter.Eq(g => g.LobbyName, lobbyName),
                        Builders<GameLobby>.Filter.Eq(g => g.Version, _version)
                    );
                    var foundGames = await _gameLobbyCollection.FindAsync(filter, cancellationToken: cancellationToken);
                    var lobby = await foundGames.FirstOrDefaultAsync(cancellationToken);
                    Console.WriteLine($"Found lobby with name: {lobbyName}");

                    var hash = GetHash(password, lobby.PasswordSalt);
                    if (hash != lobby.PasswordHash)
                    {
                        return  Result.Fail<GameLobby>("Password doesn't match");
                    }

                    var currentPlayer = _multiplayerPlayerService.LoggedInPlayer;
                    Console.WriteLine($"Creating join request for lobby: {lobbyName}");
                    var gameJoin = new GameLobbyJoin
                    {
                        GameId = lobby.Id,
                        PlayerId = currentPlayer?.Id ?? null,
                        Map = GetMapData(myMap),
                    };
                    await _gameJoinsCollection.InsertOneAsync(gameJoin, cancellationToken: cancellationToken);
                    Console.WriteLine($"Created join request with id: {gameJoin.Id}");
                    
                    Console.WriteLine("Waiting for join request confirmation");
                    var lobbyUpdate = await WaitForLobbyAccept(lobby.Id, cancellationToken);
                    if (lobbyUpdate is null || lobbyUpdate.JoinedId != gameJoin.Id)
                    {
                        return Result.Fail("The lobby is already full");
                    }
                    Console.WriteLine($"Confirmed join request with id: {gameJoin.Id}");

                    await DeleteGameAsync(lobby.Id);
                    return Result.Ok(lobby);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail("Failed to join lobby");
                }
            });
        }
    }

    private async Task<GameLobbyJoin?> WaitForGameJoinAsync(string gameId, CancellationToken cancellationToken)
    {
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameLobbyJoin>>()
            .Match(Builders<ChangeStreamDocument<GameLobbyJoin>>.Filter
                .Eq(cs => cs.FullDocument.GameId, gameId));

        using var cursor = await _gameJoinsCollection.WatchAsync(
            pipeline,
            new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
            cancellationToken
        );

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var change in cursor.Current)
            {
                if (change.FullDocument.GameId == gameId)
                {
                    return change.FullDocument;
                }
            }
        }

        return null;
    }

    private async Task<GameLobby?> WaitForLobbyAccept(string lobbyId, CancellationToken cancellationToken)
    {
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameLobby>>()
            .Match(change =>
                (change.OperationType == ChangeStreamOperationType.Update || change.OperationType == ChangeStreamOperationType.Delete) &&
                change.DocumentKey["_id"] == ObjectId.Parse(lobbyId));

        
        using var cursor = await _gameLobbyCollection.WatchAsync(
            pipeline,
            new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
            cancellationToken
        );

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var change in cursor.Current)
            {
                if (change.OperationType == ChangeStreamOperationType.Delete)
                {
                    return null;
                }
                
                if (change.FullDocument.Id == lobbyId)
                {
                    return change.FullDocument;
                }
            }
        }

        return null;
    }


    private static byte[] GetMapData(MapBlueprint map)
    {
        var myMapData = new byte[32];
        for (var i = 0; i < map.Figures.Length; i++)
        {
            var index = i * 2;
            myMapData[index] = (byte)(map.Figures[i].PlayerId + (map.Figures[i].IsKing ? 128 : 0));
            myMapData[index + 1] = (byte)(map.Figures[i].FigureId);
        }

        return myMapData;
    }

    private static string GetSalt()
    {
        var salt = RandomNumberGenerator.GetBytes(16); // Generate 16-byte salt
        return Convert.ToBase64String(salt);
    }

    private static string GetHash(string password, string saltString)
    {
        if (string.IsNullOrEmpty(password))
            return string.Empty;
        
        var unmanagedString = IntPtr.Zero;
        try
        {
            var salt = Convert.FromBase64String(saltString);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32); // 256-bit hash

            return Convert.ToBase64String(hash);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString); // Clear memory
        }
    }

    private async Task DeleteGameAsync(string gameId)
    {
        try
        {
            Console.WriteLine("Deleting LobbyJoins");
            var filter = Builders<GameLobbyJoin>.Filter.Eq(gj => gj.GameId, gameId);
            var result = await _gameJoinsCollection.DeleteManyAsync(filter);
            Console.WriteLine($"Deleted LobbyJoins: {result.DeletedCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
                
        try
        {
            Console.WriteLine("Deleting Lobbies");
            var filter = Builders<GameLobby>.Filter.Eq(gj => gj.Id, gameId);
            var result = await _gameLobbyCollection.DeleteManyAsync(filter);
            Console.WriteLine($"Deleted Lobbies: {result.DeletedCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}