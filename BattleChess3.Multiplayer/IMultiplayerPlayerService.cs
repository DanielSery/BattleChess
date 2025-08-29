using BattleChess3.Game.Players;
using BattleChess3.Maps;
using BattleChess3.Multiplayer.Tables;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerPlayerService
{
    public static byte[] DefaultUnlockedFigures { get; } = [129, 130, 193, 19, 64, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

    event EventHandler? LoggedInPlayerChanged;
    
    RegisteredPlayer? LoggedInPlayer { get; }

    Task<List<PublicPlayerData>> GetLeaderboard(CancellationToken cancellationToken);
    IOnlinePlayerInfo GetCurrentPlayer();
    Task<Result<IOnlinePlayerInfo>> GetOpponentPlayerAsync(string playerId, CancellationToken cancellationToken);
    Task<Result<string>> GetUserSaltAsync(string name, CancellationToken cancellationToken);
    Task<Result> TryLoginAsync(string name, string hash, CancellationToken cancellationToken);
    Task<Result> TrySignUpAsync(string name, string hash, string salt, string emailHash, MapBlueprint myMap, CancellationToken cancellationToken);
    Task<Result> UpdateCurrentPlayerMapAsync(MapBlueprint map, CancellationToken cancellationToken);
    Task<Result> UpdateCurrentPlayerUnlockedFigure(int unlockedFigureId, CancellationToken cancellationToken);
    Task<Result> TryVerifyEmailAsync(string emailHash, CancellationToken cancellationToken);
}