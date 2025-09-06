using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Database.Lobby;

public interface IGameLobbiesCollectionHandler
{
    Task<Result<List<PublicLobbyData>>> GetPublicLobbiesAsync(CancellationToken cancellationToken);

    Task<Result<GameLobby>> FindByNameAsync(string lobbyName, CancellationToken cancellationToken);

    Task<Result<GameLobby>> FindByIdAsync(string lobbyId, CancellationToken cancellationToken);

    Task<Result<GameLobby>> WaitForLobbyAcceptAsync(string lobbyId, CancellationToken cancellationToken);

    Task<Result> DeleteGameLobbiesAsync(string gameId, CancellationToken cancellationToken);

    Task<Result> UpdateLobbyJoinAsync(string lobbyId, string joinId, CancellationToken cancellationToken);

    Task<Result> InsertAsync(GameLobby game, CancellationToken cancellationToken);

    Task WatchChangesAsync(Func<ChangeStreamDocument<GameLobby>, Task> onLobbyChange, CancellationToken cancellationToken);
}