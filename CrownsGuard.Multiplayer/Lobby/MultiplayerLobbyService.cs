using System.Security;
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

        async Task ProcessLobbyChange((ChangeStreamOperationType operationType, string lobbyId, GameLobby? lobby) change)
        {
            switch (change.operationType)
            {
                case ChangeStreamOperationType.Insert:
                    onLobbyAdded.Invoke(new PublicLobbyData
                    {
                        Id = change.lobby!.Id,
                        LobbyName = change.lobby.LobbyName,
                        Elo = change.lobby.Elo,
                        JoinedId = change.lobby.JoinedId,
                        Locked = (change.lobby.PasswordHash.Length > 0) ? "True" : "False",
                    });
                    break;
                
                case ChangeStreamOperationType.Replace:
                case ChangeStreamOperationType.Update:
                    var foundLobbyResult = await _gameLobbies.FindLobbyByIdAsync(change.lobbyId, cancellationToken);
                    if (foundLobbyResult.TryGetValue(out var foundLobby))
                    {
                        onLobbyChanged.Invoke(new PublicLobbyData
                        {
                            Id = change.lobbyId,
                            LobbyName = foundLobby.LobbyName,
                            Elo = foundLobby.Elo,
                            JoinedId = foundLobby.JoinedId,
                            Locked = (foundLobby.PasswordHash.Length > 0) ? "True" : "False",
                        });
                    }
                    break;

                case ChangeStreamOperationType.Delete:
                    onLobbyRemoved.Invoke(change.lobbyId);
                    break;
            }
        }
    }

    public async Task<Result<GameLobby>> CreateLobbyAsync(
        string lobbyName,
        SecureString password,
        Figure[] myMap,
        CancellationToken cancellationToken)
    {
        var unlockedFigures = _multiplayerPlayerService.LoggedInPlayer?.UnlockedFigures;
        var setupValidation = myMap.ValidateResult(unlockedFigures);
        if (setupValidation.IsFailed) return setupValidation;

        var foundLobby = await _gameLobbies.FindLobbyByNameAsync(lobbyName, cancellationToken);
        if (foundLobby.IsSuccess) return Result.Fail("Lobby already exists");

        var currentPlayer = _multiplayerPlayerService.LoggedInPlayer;
        var salt = HashingHelper.GetSalt();
        var hash = HashingHelper.GetHash(password, salt);

        var random = new Random();
        var isHostStarting = random.Next(0, 2) == 1;
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
        if (!joinResult.TryGetValue(out var join))
        {
            await DeleteGameAsync(lobby.Id);
            return joinResult;
        }

        var updateResult = await _gameLobbies.ConfirmLobbyJoinAsync(lobby.Id, join.Id, cancellationToken);
        if (updateResult.IsFailed)
        {
            await DeleteGameAsync(lobby.Id);
            return Result.Fail("Failed to update game confirmation");
        }

        return join;
    }

    public async Task<Result<GameLobby>> JoinLobbyAsync(
        string lobbyName,
        SecureString password,
        Figure[] myMap,
        CancellationToken cancellationToken)
    {
        var unlockedFigures = _multiplayerPlayerService.LoggedInPlayer?.UnlockedFigures;
        var setupValidation = myMap.ValidateResult(unlockedFigures);
        if (setupValidation.IsFailed) return setupValidation;

        var foundLobbyResult = await _gameLobbies.FindLobbyByNameAsync(lobbyName, cancellationToken: cancellationToken);
        if (!foundLobbyResult.TryGetValue(out var lobby)) return Result.Fail<GameLobby>("Lobby not found");

        var hash = HashingHelper.GetHash(password, lobby.PasswordSalt);
        if (hash != lobby.PasswordHash)
        {
            return Result.Fail<GameLobby>("Password does not match");
        }

        var currentPlayer = _multiplayerPlayerService.LoggedInPlayer;
        var gameJoin = new GameLobbyJoin
        {
            GameId = lobby.Id,
            PlayerId = currentPlayer?.Id ?? null,
            Map = myMap.GetIntData(),
        };
        var gameJoinResult = await _lobbyJoins.InsertLobbyJoinAsync(gameJoin, cancellationToken);
        if (gameJoinResult.IsFailed) return Result.Fail<GameLobby>("Failed to request lobby join");

        var lobbyUpdateResult = await _gameLobbies.WaitForLobbyJoinConfirmationAsync(lobby.Id, cancellationToken);
        if (!lobbyUpdateResult.TryGetValue(out var lobbyUpdate)) return lobbyUpdateResult;
        
        if (lobbyUpdate.JoinedId is null)
        {
            await DeleteGameAsync(lobby.Id);
            return Result.Fail("The lobby was invalid");
        }

        if (lobbyUpdate.JoinedId != gameJoin.Id)
        {
            return Result.Fail("The lobby is already full");
        }

        await DeleteGameAsync(lobby.Id);
        return lobby;
    }

    private async Task DeleteGameAsync(string gameId)
    {
        await _lobbyJoins.DeleteGameJoinsAsync(gameId, CancellationToken.None);
        await _gameLobbies.DeleteGameLobbiesAsync(gameId, CancellationToken.None);
    }
}