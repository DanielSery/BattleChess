using CrownsGuard.Core.GameBoard;
using CrownsGuard.Database.Lobby;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Scheduling;
using CrownsGuard.Multiplayer.Utilities;
using FluentResults;
using MongoDB.Driver;

namespace CrownsGuard.Multiplayer.Lobby;

internal class MultiplayerLobbyService : IMultiplayerLobbyService
{
    private readonly IMultiplayerScheduler _scheduler;
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    private readonly IGameLobbiesCollectionHandler _gameLobbiesCollectionHandler;
    private readonly IGameLobbyJoinsCollectionHandler _gameLobbyJoinsCollectionHandler;

    public MultiplayerLobbyService(
        IMultiplayerScheduler scheduler,
        IMultiplayerPlayerService multiplayerPlayerService,
        IGameLobbiesCollectionHandler gameLobbiesCollectionHandler,
        IGameLobbyJoinsCollectionHandler gameLobbyJoinsCollectionHandler)
    {
        _scheduler = scheduler;
        _multiplayerPlayerService = multiplayerPlayerService;
        _gameLobbiesCollectionHandler = gameLobbiesCollectionHandler;
        _gameLobbyJoinsCollectionHandler = gameLobbyJoinsCollectionHandler;
    }

    public Task<Result<List<PublicLobbyData>>> GetPublicLobbiesAsync(CancellationToken cancellationToken)
    { 
        return _gameLobbiesCollectionHandler.GetPublicLobbiesAsync(cancellationToken);
    }
    
    public Task WatchLobbiesAsync(
        Action<PublicLobbyData> onLobbyAdded,
        Action<PublicLobbyData> onLobbyChanged,
        Action<string> onLobbyRemoved,  // pass removed lobby Id as string
        CancellationToken cancellationToken)
    {
        return _gameLobbiesCollectionHandler.WatchChangesAsync(ProcessLobbyChange, cancellationToken);

        async Task ProcessLobbyChange(ChangeStreamDocument<GameLobby> change)
        {
            switch (change.OperationType)
            {
                case ChangeStreamOperationType.Insert:
                    if (change.FullDocument != null)
                    {
                        onLobbyAdded.Invoke(new PublicLobbyData
                        {
                            Id = change.FullDocument.Id,
                            LobbyName = change.FullDocument.LobbyName,
                            Elo = change.FullDocument.Elo,
                            JoinedId = change.FullDocument.JoinedId,
                            Locked = (change.FullDocument.PasswordHash.Length > 0) ? "True" : "False",
                        });
                    }
                    break;

                case ChangeStreamOperationType.Replace:
                case ChangeStreamOperationType.Update:
                    PublicLobbyData? lobby;
                    if (change.FullDocument != null)
                    {
                        lobby = new PublicLobbyData
                        {
                            Id = change.FullDocument.Id,
                            LobbyName = change.FullDocument.LobbyName,
                            Elo = change.FullDocument.Elo,
                            JoinedId = change.FullDocument.JoinedId,
                            Locked = (change.FullDocument.PasswordHash.Length > 0) ? "True" : "False",
                        };
                    }
                    else
                    {
                        var id = change.DocumentKey["_id"].AsObjectId.ToString();
                        var foundLobbyResult = await _gameLobbiesCollectionHandler.FindByIdAsync(id, cancellationToken);
                        if (foundLobbyResult.TryGetValue(out var foundLobby))
                        {
                            lobby = new PublicLobbyData
                            {
                                Id = id,
                                LobbyName = foundLobby.LobbyName,
                                Elo = foundLobby.Elo,
                                JoinedId = foundLobby.JoinedId,
                                Locked = (foundLobby.PasswordHash.Length > 0) ? "True" : "False",
                            };
                        }
                        else
                        {
                            lobby = null;
                        }
                    }

                    if (lobby is not null)
                        onLobbyChanged.Invoke(lobby);
                    break;

                case ChangeStreamOperationType.Delete:
                    var removedId = change.DocumentKey["_id"].AsObjectId.ToString();
                    onLobbyRemoved.Invoke(removedId);
                    break;
            }
        }
    }

    public Task<Result<GameLobby>> CreateLobbyAsync(
        string lobbyName,
        string password,
        BoardBlueprint myMap,
        CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            var random = new Random();
            var isHostStarting = random.Next(0, 1) == 1;
            
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    var unlockedFigures = _multiplayerPlayerService.LoggedInPlayer?.UnlockedFigures ?? IMultiplayerPlayerService.DefaultUnlockedFigures;
                    if (!myMap.IsValid(unlockedFigures))
                        return Result.Fail<GameLobby>("Setup has units which weren't unlocked yet");

                    var foundLobby = _gameLobbiesCollectionHandler.FindByNameAsync(lobbyName, cancellationToken);
                    if (foundLobby.IsCompleted) return Result.Fail("Lobby already exists");

                    var currentPlayer = _multiplayerPlayerService.LoggedInPlayer;
                    var salt = HashingHelper.GetSalt();
                    var hash = HashingHelper.GetHash(password, salt);
                    
                    var game = new GameLobby
                    {
                        LobbyName = lobbyName,
                        PasswordHash = hash,
                        PasswordSalt = salt,
                        Map = myMap.GetByteData(),
                        PlayerId = currentPlayer?.Id ?? null,
                        Elo = currentPlayer?.Elo ?? null,
                        IsHostStarting = isHostStarting,
                    };
                    var insertResult = await _gameLobbiesCollectionHandler.InsertAsync(game, cancellationToken);
                    if (insertResult.IsFailed) return Result.Fail("Failed to insert game lobby");
                    return game;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail("Failed to create lobby");
                }
            });
        }
    }

    public Task<Result<GameLobbyJoin>> WaitForLobbyPlayerAsync(GameLobby lobby, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    Console.WriteLine($"Waiting for join request for game: {lobby.Id}");

                    var joinResult = await _gameLobbyJoinsCollectionHandler.WaitForJoinAsync(lobby.Id, cancellationToken);
                    if (!joinResult.TryGetValue(out var join)) return joinResult;

                    Console.WriteLine($"Found join request: {join.Id}");

                    var updateResult = await _gameLobbiesCollectionHandler.UpdateLobbyJoinAsync(lobby.Id, join.Id, cancellationToken);
                    if (updateResult.IsFailed)
                    {
                        await DeleteGameAsync(lobby.Id);
                        return Result.Fail("Failed to update game confirmation");
                    }

                    return Result.Ok(join);
                }
                finally
                {
                    if (lobby.JoinedId is null)
                    {
                        await DeleteGameAsync(lobby.Id);
                    }
                }
            });
        }
    }

    public Task<Result<GameLobby>> JoinLobbyAsync(
        string lobbyName,
        string password,
        BoardBlueprint myMap,
        CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                try
                {
                    var unlockedFigures = _multiplayerPlayerService.LoggedInPlayer?.UnlockedFigures ?? IMultiplayerPlayerService.DefaultUnlockedFigures;
                    if (!myMap.IsValid(unlockedFigures))
                        return Result.Fail<GameLobby>("Setup has units which weren't unlocked yet");
                    
                    var foundLobbyResult = await _gameLobbiesCollectionHandler.FindByNameAsync(lobbyName, cancellationToken: cancellationToken);
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
                        Map = myMap.GetByteData(),
                    };
                    var gameJoinResult = await _gameLobbyJoinsCollectionHandler.InsertAsync(gameJoin, cancellationToken);
                    if (gameJoinResult.IsFailed) return Result.Fail<GameLobby>("Failed to join game");

                    var lobbyUpdateResult = await _gameLobbiesCollectionHandler.WaitForLobbyAcceptAsync(lobby.Id, cancellationToken);
                    if (!lobbyUpdateResult.TryGetValue(out var lobbyUpdate)) return lobbyUpdateResult;
                    if (lobbyUpdate.JoinedId is null)
                    {
                        await DeleteGameAsync(lobby.Id);
                        return Result.Fail("The lobby was invalid");
                    }

                    await DeleteGameAsync(lobby.Id);
                    return Result.Ok(lobby);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    return Result.Fail("Failed to join lobby");
                }
            });
        }
    }

    private async Task DeleteGameAsync(string gameId)
    {
        await _gameLobbyJoinsCollectionHandler.DeleteGameJoinsAsync(gameId, CancellationToken.None);
        await _gameLobbiesCollectionHandler.DeleteGameLobbiesAsync(gameId, CancellationToken.None);
    }
}