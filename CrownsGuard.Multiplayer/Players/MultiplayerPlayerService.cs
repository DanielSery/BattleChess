using System.Collections;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Database.Players;
using CrownsGuard.Multiplayer.Scheduling;
using CrownsGuard.Multiplayer.Utilities;
using FluentResults;

namespace CrownsGuard.Multiplayer.Players;

internal class MultiplayerPlayerService : IMultiplayerPlayerService
{
    private readonly IMultiplayerScheduler _scheduler;
    private readonly IPlayersCollectionHandler _players;

    public MultiplayerPlayerService(
        IMultiplayerScheduler scheduler,
        IPlayersCollectionHandler players)
    {
        _scheduler = scheduler;
        _players = players;
    }

    /// <inheritdoc />
    public event EventHandler? LoggedInPlayerChanged;
    
    public RegisteredPlayer? LoggedInPlayer { get; private set; }

    /// <inheritdoc />
    public Task<Result<List<PublicPlayerData>>> GetLeaderboard(CancellationToken cancellationToken)
    {
        return LoggedInPlayer is null
            ? _players.GetTopLeaderboardAsync(cancellationToken)
            : _players.GetUserLeaderboardAsync(LoggedInPlayer.Id, cancellationToken);
    }

    public IOnlinePlayerInfo GetCurrentPlayer()
    {
        return LoggedInPlayer is null
            ? new ControlledOnlinePlayerInfo(Player.White, "Red player", null, null)
            : new ControlledOnlinePlayerInfo(Player.White, LoggedInPlayer.Name, LoggedInPlayer.Id, LoggedInPlayer.Elo);
    }

    public Task<Result<IOnlinePlayerInfo>> GetRemotePlayerAsync(string playerId, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                var foundPlayerResult = await _players.FindByIdAsync(playerId, cancellationToken);
                if (!foundPlayerResult.TryGetValue(out var foundPlayer)) return Result.Fail("Could not find remote player");
                return Result.Ok<IOnlinePlayerInfo>(new RemoteOnlinePlayerInfo(Player.Black, foundPlayer.Name, playerId, foundPlayer.Elo));
            });
        }
    }

    public Task<Result<string>> GetUserSaltAsync(string name, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                var foundPlayerResult = await _players.FindByNameAsync(name, cancellationToken);
                if (!foundPlayerResult.TryGetValue(out var foundPlayer)) return Result.Fail("Could not find user");
                return Result.Ok(foundPlayer.PasswordSalt);
            });
        }
    }

    public Task<Result> TryLoginAsync(string name, string hash, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                var foundPlayerResult = await _players.FindByNameAsync(name, cancellationToken);
                if (!foundPlayerResult.TryGetValue(out var foundPlayer)) return Result.Fail("Incorrect username or password");
                if (foundPlayer.PasswordHash != hash)  return Result.Fail("Incorrect username or password");
                LoggedInPlayer = foundPlayer;
                LoggedInPlayerChanged?.Invoke(this, EventArgs.Empty);
                return Result.Ok();
            });
        }
    }

    /// <inheritdoc />
    public Task<Result> UpdateCurrentPlayerMapAsync(BoardBlueprint map, CancellationToken cancellationToken)
    {
        if (LoggedInPlayer is null)
            return Task.FromResult(Result.Fail("No logged in player"));
        
        if (!map.IsValid(LoggedInPlayer.UnlockedFigures))
            return Task.FromResult(Result.Fail("Trying to save setup with not unlocked figures"));

        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                var mapData = map.GetByteData();
                var result = await _players.UpdateSetupAsync(LoggedInPlayer.Id, mapData, cancellationToken);
                if (result.IsFailed) return result;
                LoggedInPlayer.Map = mapData;
                LoggedInPlayerChanged?.Invoke(this, EventArgs.Empty);
                return result;
            });
        }
    }

    /// <inheritdoc />
    public Task<Result> UpdateCurrentPlayerUnlockedFigure(int unlockedFigureId, CancellationToken cancellationToken)
    {
        if (LoggedInPlayer is null)
            return Task.FromResult(Result.Fail("No logged in player"));

        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                var unlockedFiguresArray = new BitArray(LoggedInPlayer.UnlockedFigures)
                {
                    [unlockedFigureId] = true
                };

                var newUnlockedFigures = new byte[LoggedInPlayer.UnlockedFigures.Length];
                unlockedFiguresArray.CopyTo(newUnlockedFigures, 0);

                var result = await _players.UpdateUnlockedFiguresAsync(LoggedInPlayer.Id, newUnlockedFigures, cancellationToken);
                if (result.IsFailed) return result;
                LoggedInPlayer.UnlockedFigures = newUnlockedFigures;
                LoggedInPlayerChanged?.Invoke(this, EventArgs.Empty);
                return result;
            });
        }
    }

    /// <inheritdoc />
    public Task<Result> TryVerifyEmailAsync(string emailHash, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                var foundPlayerResult = await _players.FindByEmailHashAsync(emailHash, cancellationToken);
                if (!foundPlayerResult.IsFailed) return Result.Fail("User with given email already exists");
                return Result.Ok();
            });
        }
    }

    public Task<Result> TrySignUpAsync(string name, string hash, string salt, string emailHash, BoardBlueprint myMap, CancellationToken cancellationToken)
    {
        lock (_scheduler.SyncLock)
        {
            return _scheduler.QueueTask(async () =>
            {
                var foundPlayerResult = await _players.FindByEmailHashAsync(emailHash, cancellationToken);
                if (!foundPlayerResult.IsFailed) Result.Fail("User with given email already exists");

                foundPlayerResult = await _players.FindByNameAsync(name, cancellationToken);
                if (!foundPlayerResult.IsFailed) Result.Fail("User with given name already exists");

                var mapData = myMap.IsValid(IMultiplayerPlayerService.DefaultUnlockedFigures)
                    ? myMap.GetByteData()
                    : BoardBlueprint.ChessTeam.GetByteData();

                Console.WriteLine($"Creating new player with name: {name}");
                var player = new RegisteredPlayer
                {
                    Name = name,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    EmailHash = emailHash,
                    Elo = 1000,
                    Map = mapData,
                    UnlockedFigures = IMultiplayerPlayerService.DefaultUnlockedFigures,
                };

                return await _players.InsertAsync(player, cancellationToken);
            });
        }
    }
}