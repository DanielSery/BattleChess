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
    public static readonly int TurnTimeoutSeconds = TurnTimeout.Seconds;

    public event EventHandler<(byte, byte, TimeSpan)>? RequestPlayMove;

    public void StartGame(MultiplayerGameType gameType, string? rankedGameId);
    public Task<Result> HandleRemotePlayerTurnAsync();
    public Task<Result<string?>> HandleWinAsync(bool notifyOther, WinType winType, IOnlinePlayerInfo won, IOnlinePlayerInfo lost, CancellationToken cancellationToken);
    public Task<Result> PlayedMoveAsync(byte from, byte to, TimeSpan timeSpent, CancellationToken cancellationToken);
}
