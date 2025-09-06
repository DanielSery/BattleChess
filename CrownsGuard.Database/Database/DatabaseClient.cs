using CrownsGuard.Database.Game;
using CrownsGuard.Database.Lobby;
using CrownsGuard.Database.Players;
using CrownsGuard.Database.Ranked;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CrownsGuard.Database.Database;

internal class DatabaseClient : IDatabaseClient
{
    private IMongoDatabase? _adminDatabase;
    private IMongoCollection<GameTurn>? _gameTurns;
    private IMongoCollection<GameLobbyJoin>? _gameJoins;
    private IMongoCollection<GameLobby>? _gameLobbies;
    private IMongoCollection<RankedGameJoin>? _rankedGameJoins;
    private IMongoCollection<RankedGame>? _rankedGames;
    private IMongoCollection<RegisteredPlayer>? _players;

    public DatabaseClient()
    {
        ConnectToDatabase();
    }

    public IMongoCollection<GameTurn> GameTurns
    {
        get
        {
            if (_gameTurns == null) ConnectToDatabase();
            return _gameTurns!;
        }
    }

    public IMongoCollection<GameLobbyJoin> LobbyGameJoins
    {
        get
        {
            if (_gameJoins == null) ConnectToDatabase();
            return _gameJoins!;
        }
    }

    public IMongoCollection<GameLobby> GameLobbies
    {
        get
        {
            if (_gameLobbies == null) ConnectToDatabase();
            return _gameLobbies!;
        }
    }

    public IMongoCollection<RankedGameJoin> RankedGameJoins
    {
        get
        {
            if (_rankedGameJoins == null) ConnectToDatabase();
            return _rankedGameJoins!;
        }
    }

    public IMongoCollection<RankedGame> RankedGames
    {
        get
        {
            if (_rankedGames == null) ConnectToDatabase();
            return _rankedGames!;
        }
    }

    public IMongoCollection<RegisteredPlayer> Players
    {
        get
        {
            if (_players == null) ConnectToDatabase();
            return _players!;
        }
    }

    private void ConnectToDatabase()
    {
        try
        {
            var settings = MongoClientSettings.FromConnectionString(Secrets.ConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);

            var database = client.GetDatabase("BattleChess");
            _rankedGames = database.GetCollection<RankedGame>("RankedGames");
            _rankedGameJoins = database.GetCollection<RankedGameJoin>("RankedGameJoins");
            _gameLobbies = database.GetCollection<GameLobby>("GameLobbies");
            _gameJoins = database.GetCollection<GameLobbyJoin>("GameLobbyJoins");
            _gameTurns = database.GetCollection<GameTurn>("GameTurns");
            _players = database.GetCollection<RegisteredPlayer>("Players");

            _adminDatabase = client.GetDatabase("admin");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<DateTime> GetServerTimeAsync()
    {
        if (_adminDatabase == null) ConnectToDatabase();
        var command = new BsonDocument("hello", 1); // "hello" is the modern replacement for "isMaster"
        var result = await _adminDatabase!.RunCommandAsync<BsonDocument>(command);
        return result["localTime"].ToUniversalTime();
    }
}