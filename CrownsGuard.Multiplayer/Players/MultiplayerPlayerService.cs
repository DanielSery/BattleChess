using System.Collections;
using CrownsGuard.Core.Figures;
using CrownsGuard.Database.Errors;
using CrownsGuard.Database.Players;
using CrownsGuard.Game.Players;
using CrownsGuard.Multiplayer.Utilities;
using FluentResults;

namespace CrownsGuard.Multiplayer.Players;

internal class MultiplayerPlayerService : IMultiplayerPlayerService
{
    private readonly IPlayersCollectionHandler _players;

    public MultiplayerPlayerService(
        IPlayersCollectionHandler players)
    {
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
            ? new ControlledOnlinePlayerInfo(PlayerColor.White, "Red player", null, null)
            : new ControlledOnlinePlayerInfo(PlayerColor.White, LoggedInPlayer.Name, LoggedInPlayer.Id, LoggedInPlayer.Elo);
    }

    public async Task<Result<IOnlinePlayerInfo>> GetRemotePlayerAsync(string playerId, CancellationToken cancellationToken)
    {
        var foundPlayerResult = await _players.FindPlayerByIdAsync(playerId, cancellationToken);
        if (!foundPlayerResult.TryGetValue(out var foundPlayer)) return Result.Fail("Could not find remote player");
        return Result.Ok<IOnlinePlayerInfo>(new RemoteOnlinePlayerInfo(PlayerColor.Black, foundPlayer.Name, playerId, foundPlayer.Elo));
    }

    public async Task<Result<string>> GetUserSaltAsync(string name, CancellationToken cancellationToken)
    {
        var foundPlayerResult = await _players.FindPlayerByNameAsync(name, cancellationToken);
        if (!foundPlayerResult.TryGetValue(out var foundPlayer)) return Result.Fail("Could not find user");
        return Result.Ok(foundPlayer.PasswordSalt);
    }

    public async Task<Result> TryLoginAsync(string name, string hash, CancellationToken cancellationToken)
    {
        var foundPlayerResult = await _players.FindPlayerByNameAsync(name, cancellationToken);
        if (!foundPlayerResult.TryGetValue(out var foundPlayer)) return Result.Fail("Incorrect username or password");
        if (foundPlayer.PasswordHash != hash) return Result.Fail("Incorrect username or password");
        
        LoggedInPlayer = foundPlayer;
        LoggedInPlayerChanged?.Invoke(this, EventArgs.Empty);
        return Result.Ok();
    }

    /// <inheritdoc />
    public async Task<Result> UpdateCurrentPlayerMapAsync(Figure[] map, CancellationToken cancellationToken)
    {
        if (LoggedInPlayer is null)
            return Result.Fail("No logged in player");
        
        var setupValidation = map.ValidateMap(LoggedInPlayer.UnlockedFigures);
        if (setupValidation.IsFailed) return setupValidation;

        var mapData = map.GetIntData();
        var result = await _players.UpdatePlayerSetupAsync(LoggedInPlayer.Id, mapData, cancellationToken);
        if (result.IsFailed) return result;
        
        LoggedInPlayer.Map = mapData;
        LoggedInPlayerChanged?.Invoke(this, EventArgs.Empty);
        return result;
    }

    /// <inheritdoc />
    public async Task<Result> UpdateCurrentPlayerUnlockedFigure(Figure unlockedFigure, CancellationToken cancellationToken)
    {
        if (LoggedInPlayer is null)
            return Result.Fail("No logged in player");

        var unlockedFiguresArray = new BitArray(LoggedInPlayer.UnlockedFigures)
        {
            [(int)unlockedFigure] = true
        };

        var newUnlockedFigures = new byte[LoggedInPlayer.UnlockedFigures.Length];
        unlockedFiguresArray.CopyTo(newUnlockedFigures, 0);

        var result = await _players.UpdatePlayerUnlockedFiguresAsync(LoggedInPlayer.Id, newUnlockedFigures, cancellationToken);
        if (result.IsFailed) return result;
        
        LoggedInPlayer.UnlockedFigures = newUnlockedFigures;
        LoggedInPlayerChanged?.Invoke(this, EventArgs.Empty);
        return result;
    }

    /// <inheritdoc />
    public async Task<Result> TryVerifyEmailAsync(string emailHash, CancellationToken cancellationToken)
    {
        var foundPlayerResult = await _players.HasPlayerWithEmailHashAsync(emailHash, cancellationToken);
        if (foundPlayerResult.IsFailed) return foundPlayerResult.ToResult();
        return foundPlayerResult.Value ? Result.Fail("User with given email already exists") : Result.Ok();
    }

    public async Task<Result> TrySignUpAsync(string name, string hash, string salt, string emailHash, Figure[] myMap, Figure[] fallbackMap, CancellationToken cancellationToken)
    {
        var hasPlayerWithEmail = await _players.HasPlayerWithEmailHashAsync(emailHash, cancellationToken);
        if (hasPlayerWithEmail.IsFailed) return hasPlayerWithEmail.ToResult();
        if (hasPlayerWithEmail.Value) return Result.Fail("User with given email already exists");

        var foundPlayerResult = await _players.FindPlayerByNameAsync(name, cancellationToken);
        if (foundPlayerResult.IsSuccess) return Result.Fail("User with given name already exists");
        if (foundPlayerResult.IsFailed && !foundPlayerResult.HasError<NoResultsFoundError>()) 
            return foundPlayerResult.ToResult();

        var mapData = myMap.ValidateMap(UnlockedFigures.DefaultUnlockedFigures).IsSuccess
            ? myMap.GetIntData()
            : fallbackMap.GetIntData();

        var player = new RegisteredPlayer
        {
            Name = name,
            PasswordHash = hash,
            PasswordSalt = salt,
            EmailHash = emailHash,
            Elo = 1000,
            Map = mapData,
            UnlockedFigures = UnlockedFigures.DefaultUnlockedFigures.ToArray(),
        };
        return await _players.InsertPlayerAsync(player, cancellationToken);
    }
}