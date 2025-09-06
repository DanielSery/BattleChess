using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Database.Lobby;

public interface IGameLobbiesCollectionHandler
{
    Task<Result<List<PublicLobbyData>>> GetPublicLobbiesAsync(CancellationToken cancellationToken);

    Task<Result<GameLobby>> FindGameLobbyByNameAsync(string lobbyName, CancellationToken cancellationToken);

    Task<Result<GameLobby>> FindGameLobbyByIdAsync(string lobbyId, CancellationToken cancellationToken);

    Task<Result<GameLobby>> WaitForLobbyAccept(string lobbyId, CancellationToken cancellationToken);

    Task<Result> DeleteGameLobbies(string gameId, CancellationToken cancellationToken);

    Task<Result> UpdateLobbyJoin(string lobbyId, string joinId, CancellationToken cancellationToken);

    Task<Result> InsertGameLobby(GameLobby game, CancellationToken cancellationToken);

    Task WatchLobbyChanges(Func<ChangeStreamDocument<GameLobby>, Task> onLobbyChange, CancellationToken cancellationToken);
}