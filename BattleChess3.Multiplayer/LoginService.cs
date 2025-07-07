using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

internal class LoginService : ILoginService
{
    private Player? _loggedInPlayer;
    
    private readonly IMongoCollection<Player> _playersCollection;
    private readonly IMultiplayerScheduler _scheduler;

    public LoginService(IMultiplayerScheduler scheduler)
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
            var tcs = new TaskCompletionSource<Result<string>>();
            _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Getting user salt with name: {name}");
                    var filter = Builders<Player>.Filter.Eq("Name", name);
                    var foundPlayers = await _playersCollection.FindAsync(filter);
                    var foundPlayer = foundPlayers.FirstOrDefault();
                    Console.WriteLine($"Found user with name: {foundPlayer.Name}");
                    tcs.SetResult(Result.Ok(foundPlayer.PasswordSalt));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    tcs.SetResult(Result.Fail(ex.ToString()));
                }
            });
            
            return tcs.Task;
        }
    }

    public Task<Result> TryLogin(string name, string hash)
    {
        lock (_scheduler.SyncLock)
        {
            var tcs = new TaskCompletionSource<Result>();
            _scheduler.QueueTask(async () =>
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
                        tcs.SetResult(Result.Fail("Incorrect username or password"));
                    }
                    else
                    {
                        Console.WriteLine($"Found player: {foundPlayer.Name}");
                        _loggedInPlayer = foundPlayer;
                        tcs.SetResult(Result.Ok());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    tcs.SetResult(Result.Fail($"Failed to login as: {name}"));
                }
            });
            
            return tcs.Task;
        }
    }

    public Task<Result> TrySignUp(string name, string hash, string salt)
    {
        lock (_scheduler.SyncLock)
        {
            var tcs = new TaskCompletionSource<Result>();
            _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Getting users with name: {name}");
                    var filter = Builders<Player>.Filter.Eq("Name", name);
                    var foundPlayers = await _playersCollection.FindAsync(filter);
                    if (await foundPlayers.AnyAsync())
                    {
                        Console.WriteLine($"Found user with name: {name}");
                        tcs.SetResult(Result.Fail($"User with name {name} already exists"));
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
                    tcs.SetResult(Result.Ok());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    tcs.SetResult(Result.Fail($"Failed to create player: {name}"));
                }
            });
            
            return tcs.Task;
        }
    }
}