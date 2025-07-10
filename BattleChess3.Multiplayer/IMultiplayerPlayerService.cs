using BattleChess3.Game.Players;
using BattleChess3.Multiplayer.Tables;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerPlayerService
{
    RegisteredPlayer? LoggedInPlayer { get; }

    Task<List<PublicPlayerData>> GetLeaderboard(CancellationToken cancellationToken);
    Player GetCurrentPlayer();
    Task<Result<Player>> GetOpponentPlayerAsync(string playerId, CancellationToken cancellationToken);
    Task<Result<string>> GetUserSaltAsync(string name, CancellationToken cancellationToken);
    Task<Result> TryLoginAsync(string name, string hash, CancellationToken cancellationToken);
    Task<Result> TrySignUpAsync(string name, string hash, string salt, CancellationToken cancellationToken);
}