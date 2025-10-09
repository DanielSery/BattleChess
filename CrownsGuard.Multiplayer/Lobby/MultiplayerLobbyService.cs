using CrownsGuard.Core.Figures;
using CrownsGuard.Database.Lobby;
using CrownsGuard.Database.Utilities;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Utilities;
using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Multiplayer.Lobby;

internal class MultiplayerLobbyService : IMultiplayerLobbyService
{
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    private readonly IGameLobbiesCollectionHandler _gameLobbies;
    private readonly IGameLobbyJoinsCollectionHandler _lobbyJoins;

    public MultiplayerLobbyService(
        IMultiplayerPlayerService multiplayerPlayerService,
        IGameLobbiesCollectionHandler gameLobbies,
        IGameLobbyJoinsCollectionHandler lobbyJoins)
    {
        _multiplayerPlayerService = multiplayerPlayerService;
        _gameLobbies = gameLobbies;
        _lobbyJoins = lobbyJoins;
    }

    public Task<Result<List<PublicLobbyData>>> GetPublicLobbiesAsync(CancellationToken cancellationToken)
    { 
        return _gameLobbies.GetPublicLobbiesAsync(cancellationToken);
    }
    
    public Task WatchLobbiesAsync(
        Action<PublicLobbyData> onLobbyAdded,
        Action<PublicLobbyData> onLobbyChanged,
        Action<string> onLobbyRemoved,  // pass removed lobby Id as string
        CancellationToken cancellationToken)
    {
        return _gameLobbies.WatchChangesAsync(ProcessLobbyChange, cancellationToken);

        async Task ProcessLobbyChange(ChangeStreamDocument<GameLobby> change)
        {
            switch (change.OperationType)
            {
                case ChangeStreamOperationType.Insert:
                    if (change.FullDocument != null)
                        onLobbyAdded.Invoke(GetNewLobby(change.FullDocument));
                    break;
                
                case ChangeStreamOperationType.Replace:
                case ChangeStreamOperationType.Update:
                    if (change.FullDocument == null)
                    {
                        var id = change.DocumentKey["_id"].AsObjectId.ToString();
                        var foundLobbyResult = await _gameLobbies.FindLobbyByIdAsync(id, cancellationToken);
                        if (foundLobbyResult.TryGetValue(out var foundLobby))
                            onLobbyChanged.Invoke(GetExistingLobby(id, foundLobby));
                        break;
                    }
                    
                    if (change.FullDocument != null)
                        onLobbyChanged.Invoke(GetNewLobby(change.FullDocument));
                    break;

                case ChangeStreamOperationType.Delete:
                    var removedId = change.DocumentKey["_id"].AsObjectId.ToString();
                    onLobbyRemoved.Invoke(removedId);
                    break;
            }
        }

        PublicLobbyData GetNewLobby(GameLobby lobby)
        {
            return new PublicLobbyData
            {
                Id = lobby.Id,
                LobbyName = lobby.LobbyName,
                Elo = lobby.Elo,
                JoinedId = lobby.JoinedId,
                Locked = (lobby.PasswordHash.Length > 0) ? "True" : "False",
            };
        }
        
        PublicLobbyData GetExistingLobby(string id, GameLobby foundLobby)
        {
            return new PublicLobbyData
            {
                Id = id,
                LobbyName = foundLobby.LobbyName,
                Elo = foundLobby.Elo,
                JoinedId = foundLobby.JoinedId,
                Locked = (foundLobby.PasswordHash.Length > 0) ? "True" : "False",
            };
        }
    }

    public async Task<Result<GameLobby>> CreateLobbyAsync(
        string lobbyName,
        string password,
        Figure[] myMap,
        CancellationToken cancellationToken)
    {
        var random = new Random();
        var isHostStarting = random.Next(0, 1) == 1;

        var unlockedFigures = _multiplayerPlayerService.LoggedInPlayer?.UnlockedFigures;
        if (!myMap.IsValid(unlockedFigures))
            return Result.Fail<GameLobby>("Setup has units which weren't unlocked yet");

        var foundLobby = _gameLobbies.FindLobbyByNameAsync(lobbyName, cancellationToken);
        if (foundLobby.IsCompleted) return Result.Fail("Lobby already exists");

        var currentPlayer = _multiplayerPlayerService.LoggedInPlayer;
        var salt = HashingHelper.GetSalt();
        var hash = HashingHelper.GetHash(password, salt);

        var game = new GameLobby
        {
            LobbyName = lobbyName,
            PasswordHash = hash,
            PasswordSalt = salt,
            Map = myMap.GetIntData(),
            PlayerId = currentPlayer?.Id ?? null,
            Elo = currentPlayer?.Elo ?? null,
            IsHostStarting = isHostStarting,
            Version = GameVersion.VersionId
        };
        var insertResult = await _gameLobbies.InsertLobbyAsync(game, cancellationToken);
        
        if (insertResult.IsFailed) return Result.Fail("Failed to insert game lobby");
        return game;
    }

    public async Task<Result<GameLobbyJoin>> WaitForLobbyPlayerAsync(GameLobby lobby, CancellationToken cancellationToken)
    {
        var joinResult = await _lobbyJoins.WaitForLobbyJoinAsync(lobby.Id, cancellationToken);
        if (!joinResult.TryGetValue(out var join)) return joinResult;

        var updateResult = await _gameLobbies.ConfirmLobbyJoinAsync(lobby.Id, join.Id, cancellationToken);
        if (updateResult.IsFailed)
        {
            await DeleteGameAsync(lobby.Id);
            return Result.Fail("Failed to update game confirmation");
        }

        return Result.Ok(join);
    }

    public async Task<Result<GameLobby>> JoinLobbyAsync(
        string lobbyName,
        string password,
        Figure[] myMap,
        CancellationToken cancellationToken)
    {
        var unlockedFigures = _multiplayerPlayerService.LoggedInPlayer?.UnlockedFigures;
        if (!myMap.IsValid(unlockedFigures))
            return Result.Fail<GameLobby>("Setup has units which weren't unlocked yet");

        var foundLobbyResult = await _gameLobbies.FindLobbyByNameAsync(lobbyName, cancellationToken: cancellationToken);
        if (!foundLobbyResult.TryGetValue(out var lobby)) return Result.Fail<GameLobby>("Lobby not found");

        var hash = HashingHelper.GetHash(password, lobby.PasswordSalt);
        if (hash != lobby.PasswordHash)
        {
            return Result.Fail<GameLobby>("Password doesn't match");
        }

        var currentPlayer = _multiplayerPlayerService.LoggedInPlayer;
        var gameJoin = new GameLobbyJoin
        {
            GameId = lobby.Id,
            PlayerId = currentPlayer?.Id ?? null,
            Map = myMap.GetIntData(),
        };
        var gameJoinResult = await _lobbyJoins.InsertLobbyJoinAsync(gameJoin, cancellationToken);
        if (gameJoinResult.IsFailed) return Result.Fail<GameLobby>("Failed to join game");

        var lobbyUpdateResult = await _gameLobbies.WaitForLobbyJoinCofirmationAsync(lobby.Id, cancellationToken);
        if (!lobbyUpdateResult.TryGetValue(out var lobbyUpdate)) return lobbyUpdateResult;
        if (lobbyUpdate.JoinedId is null)
        {
            await DeleteGameAsync(lobby.Id);
            return Result.Fail("The lobby was invalid");
        }

        await DeleteGameAsync(lobby.Id);
        return Result.Ok(lobby);
    }

    private async Task DeleteGameAsync(string gameId)
    {
        await _lobbyJoins.DeleteGameJoinsAsync(gameId, CancellationToken.None);
        await _gameLobbies.DeleteGameLobbiesAsync(gameId, CancellationToken.None);
    }
}