using BattleChess3.Maps;
using BattleChess3.Multiplayer.Tables;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerLobbyService
{
    public Task<List<PublicLobbyData>> GetPublicLobbies();
    
    public Task<Result<GameLobby>> CreateLobby(
        string lobbyName,
        string password,
        bool isHostStarting, 
        MapBlueprint myMap);
    
    public Task<Result<GameJoin>> WaitForLobbyPlayer(GameLobby lobby);
    
    public Task<Result<GameLobby>> JoinLobby(
        string lobbyName,
        string password, 
        MapBlueprint myMap);
    
    public Task<Result> DeleteGame(string gameId);
}