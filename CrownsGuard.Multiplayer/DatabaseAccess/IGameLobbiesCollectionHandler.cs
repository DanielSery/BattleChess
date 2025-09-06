using CrownsGuard.Multiplayer.Lobby;
using CrownsGuard.Multiplayer.Tables;
using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Multiplayer.DatabaseAccess;

public interface IGameLobbiesCollectionHandler
{
    Task<Result<List<PublicLobbyData>>> GetPublicLobbiesAsync(CancellationToken cancellationToken);

    Task<Result<GameLobby>> FindGameLobbyAsync(string lobbyName, int gameVersion, CancellationToken cancellationToken);

    Task<Result<GameLobby>> FindGameLobbyAsync(string lobbyId, CancellationToken cancellationToken);

    Task<Result<GameLobby>> WaitForLobbyAccept(string lobbyId, CancellationToken cancellationToken);

    Task<Result> DeleteGameLobbies(string gameId, CancellationToken cancellationToken);

    Task<Result> UpdateLobbyJoin(string lobbyId, string joinId, CancellationToken cancellationToken);

    Task<Result> InsertGameLobby(GameLobby game, CancellationToken cancellationToken);

    Task WatchLobbyChanges(Func<ChangeStreamDocument<GameLobby>, Task> onLobbyChange, CancellationToken cancellationToken);
}