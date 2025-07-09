using BattleChess3.Maps;
using BattleChess3.Multiplayer.Tables;
using FluentResults;
using MongoDB.Bson;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerLobbyService
{
    public Task<List<PublicLobbyData>> GetPublicLobbiesAsync();

    Task WatchLobbiesAsync(
        Action<PublicLobbyData> onLobbyAdded,
        Action<PublicLobbyData> onLobbyChanged,
        Action<string> onLobbyRemoved,  // pass removed lobby Id as string
        CancellationToken cancellationToken);
    
    public Task<Result<GameLobby>> CreateLobbyAsync(
        string lobbyName,
        string password,
        MapBlueprint myMap);
    
    public Task<Result<GameLobbyJoin>> WaitForLobbyPlayerAsync(GameLobby lobby);
    
    public Task<Result<GameLobby>> JoinLobbyAsync(
        string lobbyName,
        string password, 
        MapBlueprint myMap);
    
    public Task<Result> DeleteGameAsync(string gameId);
}