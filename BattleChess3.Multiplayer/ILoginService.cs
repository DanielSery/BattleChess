using BattleChess3.Multiplayer.Tables;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface ILoginService
{
    Player? LoggedInPlayer { get; }
    
    Task<Result<string>> GetUserSalt(string name);
    Task<Result> TryLogin(string name, string hash);
    Task<Result> TrySignUp(string name, string hash, string salt);
}