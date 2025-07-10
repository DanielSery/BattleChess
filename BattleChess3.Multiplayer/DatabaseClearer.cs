using BattleChess3.Multiplayer.Tables;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

public class DatabaseClearer
{
    private readonly IMongoCollection<RankedGame> _rankedGamesCollection;
    private readonly IMongoCollection<RankedGameJoin> _rankedGameJoinsCollection;
    private readonly IMongoCollection<GameLobby> _gameLobbyCollection;
    private readonly IMongoCollection<GameLobbyJoin> _gameJoinsCollection;
    private readonly IMongoCollection<GameTurn> _gameTurnsCollection;

    public DatabaseClearer()
    {
        var client = new MongoClient(DbSecrets.ConnectionString);
        var database = client.GetDatabase("BattleChess");
        _rankedGamesCollection = database.GetCollection<RankedGame>("RankedGames");
        _rankedGameJoinsCollection = database.GetCollection<RankedGameJoin>("RankedGameJoins");
        _gameLobbyCollection = database.GetCollection<GameLobby>("GameLobbies");
        _gameJoinsCollection = database.GetCollection<GameLobbyJoin>("GameLobbyJoins");
        _gameTurnsCollection = database.GetCollection<GameTurn>("GameTurns");
    }

    public void ClearDatabase()
    {
        _rankedGamesCollection.DeleteMany(Builders<RankedGame>.Filter.Empty);
        _rankedGameJoinsCollection.DeleteMany(FilterDefinition<RankedGameJoin>.Empty);
        _gameLobbyCollection.DeleteMany(FilterDefinition<GameLobby>.Empty);
        _gameJoinsCollection.DeleteMany(FilterDefinition<GameLobbyJoin>.Empty);
        _gameTurnsCollection.DeleteMany(FilterDefinition<GameTurn>.Empty);
    }
}