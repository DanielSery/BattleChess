using BattleChess3.Multiplayer.Tables;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

public class DatabaseClient : IDatabaseClient
{
    private readonly IMongoDatabase? _adminDatabase;

    public DatabaseClient()
    {
        try
        {
            var settings = MongoClientSettings.FromConnectionString(Secrets.ConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);

            var database = client.GetDatabase("BattleChess");
            RankedGames = database.GetCollection<RankedGame>("RankedGames");
            RankedGameJoins = database.GetCollection<RankedGameJoin>("RankedGameJoins");
            GameLobbies = database.GetCollection<GameLobby>("GameLobbies");
            GameJoins = database.GetCollection<GameLobbyJoin>("GameLobbyJoins");
            GameTurns = database.GetCollection<GameTurn>("GameTurns");
            Players = database.GetCollection<RegisteredPlayer>("Players");

            _adminDatabase = client.GetDatabase("admin");

            IsConnected = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            RankedGames = null;
            RankedGameJoins = null;
            GameLobbies = null;
            GameJoins = null;
            GameTurns = null;
            Players = null;

            _adminDatabase = null;

            IsConnected = false;
        }
    }

    public bool IsConnected { get; }

    public IMongoCollection<GameTurn>? GameTurns { get; }

    public IMongoCollection<GameLobbyJoin>? GameJoins { get; }

    public IMongoCollection<GameLobby>? GameLobbies { get; }

    public IMongoCollection<RankedGameJoin>? RankedGameJoins { get; }

    public IMongoCollection<RankedGame>? RankedGames { get; }

    public IMongoCollection<RegisteredPlayer>? Players { get; }

    public async Task<DateTime> GetServerTimeAsync()
    {
        if (!IsConnected) return DateTime.MinValue;

        var command = new BsonDocument("hello", 1); // "hello" is the modern replacement for "isMaster"
        var result = await _adminDatabase!.RunCommandAsync<BsonDocument>(command);
        return result["localTime"].ToUniversalTime();
    }
}