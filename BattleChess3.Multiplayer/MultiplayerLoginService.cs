using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

internal class MultiplayerLoginService : IMultiplayerLoginService
{
    private Player? _loggedInPlayer;
    
    private readonly IMongoCollection<Player> _playersCollection;
    private readonly IMultiplayerScheduler _scheduler;

    public MultiplayerLoginService(IMultiplayerScheduler scheduler)
    {
        _scheduler = scheduler;
        var client = new MongoClient(DbSecrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _playersCollection = database.GetCollection<Player>("Players");
    }

    public Player? LoggedInPlayer => _loggedInPlayer;

    public Task<Result<string>> GetUserSalt(string name)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Getting user salt with name: {name}");
                    var filter = Builders<Player>.Filter.Eq("Name", name);
                    var foundPlayers = await _playersCollection.FindAsync(filter);
                    var foundPlayer = foundPlayers.FirstOrDefault();
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

    public Task<Result> TryLogin(string name, string hash)
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
                    var foundPlayers = await _playersCollection.FindAsync(filter);
                    var foundPlayer = foundPlayers.FirstOrDefault();
                    if (foundPlayer is null)
                    {
                        return Result.Fail("Incorrect username or password");
                    }
                    else
                    {
                        Console.WriteLine($"Found player: {foundPlayer.Name}");
                        _loggedInPlayer = foundPlayer;
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

    public Task<Result> TrySignUp(string name, string hash, string salt)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Getting users with name: {name}");
                    var filter = Builders<Player>.Filter.Eq("Name", name);
                    var foundPlayers = await _playersCollection.FindAsync(filter);
                    if (await foundPlayers.AnyAsync())
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

                    await _playersCollection.InsertOneAsync(player);
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