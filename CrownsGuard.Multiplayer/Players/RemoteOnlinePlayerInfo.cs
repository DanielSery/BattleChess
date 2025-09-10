using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Game.Timers;
using CrownsGuard.Multiplayer.Game;

namespace CrownsGuard.Multiplayer.Players;

public class RemoteOnlinePlayerInfo : IOnlinePlayerInfo, IAutomaticallyControlledPlayerInfo
{
    private IMultiplayerGameService? _gameService;

    public RemoteOnlinePlayerInfo(Core.Players.PlayerColor playerColor, string playerName, string? playerId, int? elo)
    {
        PlayerColor = playerColor;
        Name = playerName;
        PlayerId = playerId;
        Elo = elo;
        Timer = InfinitePlayerTimer.Instance;
    }

    public Core.Players.PlayerColor PlayerColor { get; }
    public ArrayPoolMemory<Figure> Board { get; private set; }

    public Figure[] PlayerBoard { get; set; } = [];
    public IPlayerTimer Timer { get; private set; }
    public string Name { get; }
    public string? PlayerId { get; }
    public int? Elo { get; }

    /// <inheritdoc />
    public void StartTurn()
    {
        Timer.StartTurnTimer();
    }

    /// <inheritdoc />
    public void EndTurn(TimeSpan? forcedTurnDuration = null)
    {
        Timer.EndTurnTimer(forcedTurnDuration);
    }

    /// <inheritdoc />
    public Task HandleAutomaticTurnAsync()
    {
        if (_gameService == null) throw new ArgumentNullException(nameof(_gameService));

        return Task.Run(_gameService.HandleRemotePlayerTurnAsync);
    }

    /// <inheritdoc />
    public void SetTimer(IPlayerTimer timer)
    {
        Timer = timer;
    }

    /// <inheritdoc />
    public void SetGameService(IMultiplayerGameService gameService)
    {
        _gameService = gameService;
    }

    /// <inheritdoc />
    public void UpdateBoard(ArrayPoolMemory<Figure> board)
    {
        Board.Dispose();
        Board = board;
    }
}