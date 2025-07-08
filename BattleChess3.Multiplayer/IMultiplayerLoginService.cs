using BattleChess3.Multiplayer.Tables;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerLoginService
{
    Player? LoggedInPlayer { get; }
    
    Task<Result<string>> GetUserSaltAsync(string name);
    Task<Result> TryLoginAsync(string name, string hash);
    Task<Result> TrySignUpAsync(string name, string hash, string salt);
}