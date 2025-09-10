using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.Database.Players;
using FluentResults;

namespace CrownsGuard.Multiplayer.Players;

public interface IMultiplayerPlayerService
{
    public static byte[] DefaultUnlockedFigures { get; } = [129, 130, 193, 19, 64, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

    event EventHandler? LoggedInPlayerChanged;
    
    RegisteredPlayer? LoggedInPlayer { get; }

    Task<Result<List<PublicPlayerData>>> GetLeaderboard(CancellationToken cancellationToken);
    IOnlinePlayerInfo GetCurrentPlayer();
    Task<Result<IOnlinePlayerInfo>> GetRemotePlayerAsync(string playerId, CancellationToken cancellationToken);
    Task<Result<string>> GetUserSaltAsync(string name, CancellationToken cancellationToken);
    Task<Result> TryLoginAsync(string name, string hash, CancellationToken cancellationToken);
    Task<Result> TrySignUpAsync(string name, string hash, string salt, string emailHash, BoardBlueprint myMap, CancellationToken cancellationToken);
    Task<Result> UpdateCurrentPlayerMapAsync(BoardBlueprint map, CancellationToken cancellationToken);
    Task<Result> UpdateCurrentPlayerUnlockedFigure(FigureId unlockedFigureId, CancellationToken cancellationToken);
    Task<Result> TryVerifyEmailAsync(string emailHash, CancellationToken cancellationToken);
}