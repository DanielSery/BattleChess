using CrownsGuard.Core.GameBoard;
using CrownsGuard.Game;
using CrownsGuard.Multiplayer.Players;
using FluentResults;

namespace CrownsGuard.Multiplayer.Game;

public interface IMultiplayerGameService
{
    // Cannot be tile index
    public const int SurrenderMessage = 67;
    public const int OutOfTimeMessage = 68;
    public const int NotRespondingMessage = 69;
    public const int NotRespondingLostMessage = 70;

    public static readonly TimeSpan TurnTimeout = TimeSpan.FromMinutes(2);

    public event EventHandler<(Position, Position, TimeSpan)>? RequestPlayMove;

    public void StartGame(MultiplayerGameType gameType, string? rankedGameId);
    public Task<Result> HandleRemotePlayerTurnAsync();
    public Task<Result<string?>> HandleWinAsync(bool notifyOther, WinType winType, IOnlinePlayerInfo won, IOnlinePlayerInfo lost);
    public Task<Result> PlayedMoveAsync(Position from, Position to, TimeSpan timeSpent);
}
