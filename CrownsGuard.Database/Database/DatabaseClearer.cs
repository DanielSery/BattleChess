using CrownsGuard.Database.Game;
using CrownsGuard.Database.Lobby;
using CrownsGuard.Database.Ranked;
using MongoDB.Driver;

namespace CrownsGuard.Database.Database;

internal class DatabaseClearer : IDatabaseClearer
{
    private readonly IMongoCollection<RankedGame> _rankedGamesCollection;
    private readonly IMongoCollection<RankedGameJoin> _rankedGameJoinsCollection;
    private readonly IMongoCollection<GameLobby> _gameLobbyCollection;
    private readonly IMongoCollection<GameLobbyJoin> _gameJoinsCollection;
    private readonly IMongoCollection<GameTurn> _gameTurnsCollection;

    internal DatabaseClearer(IDatabaseClient databaseClient)
    {
        _rankedGamesCollection = databaseClient.RankedGames!;
        _rankedGameJoinsCollection = databaseClient.RankedGameJoins!;
        _gameLobbyCollection = databaseClient.GameLobbies!;
        _gameJoinsCollection = databaseClient.GameJoins!;
        _gameTurnsCollection = databaseClient.GameTurns!;
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