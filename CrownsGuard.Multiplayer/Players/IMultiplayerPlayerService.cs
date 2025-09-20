using CrownsGuard.Core.Figures;
using CrownsGuard.Database.Players;
using FluentResults;

namespace CrownsGuard.Multiplayer.Players;

public interface IMultiplayerPlayerService
{
    event EventHandler? LoggedInPlayerChanged;
    
    RegisteredPlayer? LoggedInPlayer { get; }

    Task<Result<List<PublicPlayerData>>> GetLeaderboard(CancellationToken cancellationToken);
    IOnlinePlayerInfo GetCurrentPlayer();
    Task<Result<IOnlinePlayerInfo>> GetRemotePlayerAsync(string playerId, CancellationToken cancellationToken);
    Task<Result<string>> GetUserSaltAsync(string name, CancellationToken cancellationToken);
    Task<Result> TryLoginAsync(string name, string hash, CancellationToken cancellationToken);
    Task<Result> TrySignUpAsync(string name, string hash, string salt, string emailHash, Figure[] myMap, Figure[] fallbackMap, CancellationToken cancellationToken);
    Task<Result> UpdateCurrentPlayerMapAsync(Figure[] map, CancellationToken cancellationToken);
    Task<Result> UpdateCurrentPlayerUnlockedFigure(Figure unlockedFigure, CancellationToken cancellationToken);
    Task<Result> TryVerifyEmailAsync(string emailHash, CancellationToken cancellationToken);
}