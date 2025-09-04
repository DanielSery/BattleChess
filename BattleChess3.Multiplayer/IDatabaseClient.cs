using BattleChess3.Multiplayer.Tables;
using MongoDB.Driver;

namespace BattleChess3.Multiplayer;

public interface IDatabaseClient
{
    bool IsConnected { get; }

    IMongoCollection<GameTurn>? GameTurns { get; }

    IMongoCollection<GameLobbyJoin>? GameJoins { get; }

    IMongoCollection<GameLobby>? GameLobbies { get; }

    IMongoCollection<RankedGameJoin>? RankedGameJoins { get; }

    IMongoCollection<RankedGame>? RankedGames { get; }

    IMongoCollection<RegisteredPlayer>? Players { get; }

    Task<DateTime> GetServerTimeAsync();
}