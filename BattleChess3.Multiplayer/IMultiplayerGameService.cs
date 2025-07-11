using BattleChess3.Game.Board;
using BattleChess3.Game.Players;
using FluentResults;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerGameService
{
    public const int SurrenderMessage = 67;
    public const int OutOfTimeMessage = 68;
    public const int NotRespondingMessage = 69;
    
    public static readonly TimeSpan TurnTimeout = TimeSpan.FromMinutes(2);
    
    public event EventHandler<(Position, Position, TimeSpan)>? RequestPlayMove;

    public void StartGame(MultiplayerGameType gameType, string? rankedGameId);
    public Task<Result> HandleHisTurnAsync();
    public Task<Result<string?>> HandleWinAsync(bool notifyOther, WinType winType, Player won, Player lost);
    public Task<Result> PlayedMoveAsync(Position from, Position to, TimeSpan timeSpent);
}