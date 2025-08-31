using BattleChess3.Core.GameBoard;
using BattleChess3.Multiplayer.Tables;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerLobbyService
{
    public Task<List<PublicLobbyData>> GetPublicLobbiesAsync(
        CancellationToken cancellationToken);

    Task WatchLobbiesAsync(
        Action<PublicLobbyData> onLobbyAdded,
        Action<PublicLobbyData> onLobbyChanged,
        Action<string> onLobbyRemoved,  // pass removed lobby Id as string
        CancellationToken cancellationToken);
    
    public Task<Result<GameLobby>> CreateLobbyAsync(
        string lobbyName,
        string password,
        BoardBlueprint myMap,
        CancellationToken cancellationToken);
    
    public Task<Result<GameLobbyJoin>> WaitForLobbyPlayerAsync(
        GameLobby lobby,
        CancellationToken cancellationToken);
    
    public Task<Result<GameLobby>> JoinLobbyAsync(
        string lobbyName,
        string password, 
        BoardBlueprint myMap,
        CancellationToken cancellationToken);
}