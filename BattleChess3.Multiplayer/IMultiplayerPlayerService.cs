using BattleChess3.Multiplayer.Tables;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerPlayerService
{
    Player? LoggedInPlayer { get; }

    Task<List<PublicPlayerData>> GetLeaderboard();
    Task<Result<string>> GetUserSaltAsync(string name);
    Task<Result> TryLoginAsync(string name, string hash);
    Task<Result> TrySignUpAsync(string name, string hash, string salt);
}