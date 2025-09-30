using CrownsGuard.Database.Game;
using CrownsGuard.Database.Lobby;
using CrownsGuard.Database.Players;
using CrownsGuard.Database.Ranked;
using MongoDB.Driver;

namespace CrownsGuard.Database.Database;

public interface IDatabaseClient
{
    IMongoCollection<GameTurn> GameTurns { get; }

    IMongoCollection<GameLobbyJoin> LobbyGameJoins { get; }

    IMongoCollection<GameLobby> GameLobbies { get; }

    IMongoCollection<RankedGameJoin> RankedGameJoins { get; }

    IMongoCollection<RankedGame> RankedGames { get; }

    IMongoCollection<RegisteredPlayer> Players { get; }

    Task<DateTime> GetServerTimeAsync();
}