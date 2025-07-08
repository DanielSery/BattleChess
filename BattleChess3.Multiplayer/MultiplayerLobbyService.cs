using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using BattleChess3.Maps;
using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

public class MultiplayerLobbyService : IMultiplayerLobbyService
{
    private readonly int _version;
    
    private readonly IMultiplayerScheduler _scheduler;
    private readonly IMultiplayerLoginService _multiplayerLoginService;
    
    private readonly IMongoCollection<GameLobby> _gameLobbyCollection;
    private readonly IMongoCollection<GameLobbyJoin> _gameJoinsCollection;

    public MultiplayerLobbyService(
        IMultiplayerScheduler scheduler,
        IMultiplayerLoginService multiplayerLoginService)
    {
        _scheduler = scheduler;
        _multiplayerLoginService = multiplayerLoginService;
        
        var client = new MongoClient(DbSecrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _gameLobbyCollection = database.GetCollection<GameLobby>("GameLobbies");
        _gameJoinsCollection = database.GetCollection<GameLobbyJoin>("GameLobbyJoins");

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        _version = version is not null 
            ? ((byte)version.Major) << 16 | (byte)version.Minor << 8 | (byte)version.Revision
            : -1;
    }

    public Task<List<PublicLobbyData>> GetPublicLobbiesAsync()
    { 
        return _gameLobbyCollection.Aggregate()
            .Match(l => l.Version == _version && string.IsNullOrEmpty(l.JoinedId))
            .Project(doc => new PublicLobbyData
            {
                LobbyName = doc.LobbyName,
                Elo = doc.Elo,
                Locked = doc.PasswordHash.Length > 0,
            })
            .ToListAsync();
    }

    public Task<Result<GameLobby>> CreateLobbyAsync(
        string lobbyName,
        string password,
        MapBlueprint myMap)
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
                    var foundGames = await _gameLobbyCollection.FindAsync(filter);
                    if (await foundGames.AnyAsync())
                    {
                        return Result.Fail<GameLobby>("Lobby already exists");
                    }
                    Console.WriteLine($"Found lobby with name: {lobbyName}");
                    
                    var currentPlayer = _multiplayerLoginService.LoggedInPlayer;
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
                    await _gameLobbyCollection.InsertOneAsync(game);
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

    public Task<Result<GameLobbyJoin>> WaitForLobbyPlayerAsync(GameLobby lobby)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Waiting for join request for game: {lobby.Id}");
                    var joinRequest = await WaitForGameJoinAsync(lobby.Id, 5 * 60);
                    if (joinRequest is null)
                    {
                        await DeleteGameAsync(lobby.Id);
                        return Result.Fail("Timeout waiting for joining player");
                    }
                    Console.WriteLine($"Found join request: {joinRequest.Id}");
                    
                    Console.WriteLine("Confirming game join");
                    var filter = Builders<GameLobby>.Filter.Eq(l => l.Id, lobby.Id);
                    var update = Builders<GameLobby>.Update.Set(x => x.JoinedId, joinRequest.Id);
                    var result = await _gameLobbyCollection.UpdateOneAsync(filter, update);
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
                    await DeleteGameAsync(lobby.Id);
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail("Failed to wait for joining player");
                } 
            });
        }
    }

    public Task<Result<GameLobby>> JoinLobbyAsync(
        string lobbyName,
        string password,
        MapBlueprint myMap)
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
                    var foundGames = await _gameLobbyCollection.FindAsync(filter);
                    var gameRequest = foundGames.FirstOrDefault();
                    Console.WriteLine($"Found lobby with name: {lobbyName}");

                    var hash = GetHash(password, gameRequest.PasswordSalt);
                    if (hash != gameRequest.PasswordHash)
                    {
                        return  Result.Fail<GameLobby>("Password doesn't match");
                    }

                    var currentPlayer = _multiplayerLoginService.LoggedInPlayer;
                    Console.WriteLine($"Creating join request for lobby: {lobbyName}");
                    var gameJoin = new GameLobbyJoin
                    {
                        GameId = gameRequest.Id,
                        PlayerId = currentPlayer?.Id ?? null,
                        Map = GetMapData(myMap),
                    };
                    await _gameJoinsCollection.InsertOneAsync(gameJoin);
                    Console.WriteLine($"Created join request with id: {gameJoin.Id}");
                    
                    Console.WriteLine("Waiting for join request confirmation");
                    var lobbyUpdate = await WaitForLobbyAccept(gameRequest.Id);
                    if (lobbyUpdate is null || lobbyUpdate.JoinedId != gameJoin.Id)
                    {
                        return Result.Fail("The lobby is already full");
                    }
                    Console.WriteLine($"Confirmed join request with id: {gameJoin.Id}");
                    
                    return Result.Ok(gameRequest);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail("Failed to join lobby");
                }
            });
        }
    }

    private async Task<GameLobbyJoin?> WaitForGameJoinAsync(string gameId, int timeoutSeconds = 30)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameLobbyJoin>>()
            .Match(Builders<ChangeStreamDocument<GameLobbyJoin>>.Filter
                .Eq(cs => cs.FullDocument.GameId, gameId));

        using var cursor = await _gameJoinsCollection.WatchAsync(
            pipeline,
            new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
            cancellationTokenSource.Token
        );

        while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
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
    
    public async Task<GameLobby?> WaitForLobbyAccept(string lobbyId, int timeoutSeconds = 30)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<GameLobby>>()
            .Match(change =>
                change.OperationType == ChangeStreamOperationType.Update &&
                change.DocumentKey["_id"] == ObjectId.Parse(lobbyId));

        
        using var cursor = await _gameLobbyCollection.WatchAsync(
            pipeline,
            new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup },
            cancellationTokenSource.Token
        );

        while (await cursor.MoveNextAsync(cancellationTokenSource.Token))
        {
            foreach (var change in cursor.Current)
            {
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

    public Task<Result> DeleteGameAsync(string gameId)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine("Deleting GameJoins");
                    var filter = Builders<GameLobbyJoin>.Filter.Eq(gj => gj.GameId, gameId);
                    var result = await _gameJoinsCollection.DeleteManyAsync(filter);
                    Console.WriteLine($"Deleted GameJoins: {result.DeletedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
                try
                {
                    Console.WriteLine("Deleting GameLobbies");
                    var filter = Builders<GameLobby>.Filter.Eq(gj => gj.Id, gameId);
                    var result = await _gameLobbyCollection.DeleteManyAsync(filter);
                    Console.WriteLine($"Deleted GameLobbies: {result.DeletedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                return Result.Ok();
            });
        }
    }
}