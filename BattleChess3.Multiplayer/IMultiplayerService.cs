using BattleChess3.Game.Board;
using BattleChess3.Maps;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerService
{
    public bool IsHost { get; }
    public bool IsGuest { get; }
    public event EventHandler<(Position, Position)>? RequestPlayMove;
    public event EventHandler<MapBlueprint>? RequestLoadMap;
    public event EventHandler<string>? RequestDisplayMessage;

    Task<Result<string>> GetUserSalt(string name);
    Task<Result> TryLogin(string name, string hash);
    Task<Result> TrySignUp(string name, string hash, string salt);
    public Task<string> Host(bool isPublic, bool isHostStarting, MapBlueprint myMap);
    public Task WaitForHostConfirmation(bool isHostStarting, MapBlueprint myMap);
    public void Join(string gameId, MapBlueprint myMap);
    public void Stop();
    public void PlayedMove(Position from, Position to);
}