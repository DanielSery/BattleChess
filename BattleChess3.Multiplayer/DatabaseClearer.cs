using BattleChess3.Multiplayer.DatabaseAccess;
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

    public DatabaseClearer(IDatabaseClient databaseClient)
    {
        _rankedGamesCollection = databaseClient.RankedGames;
        _rankedGameJoinsCollection = databaseClient.RankedGameJoins;
        _gameLobbyCollection = databaseClient.GameLobbies;
        _gameJoinsCollection = databaseClient.GameJoins;
        _gameTurnsCollection = databaseClient.GameTurns;
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