using CrownsGuard.Core.Figures;
using CrownsGuard.Game.Players;

namespace CrownsGuard.Game;

public interface IGameService
{
    public event EventHandler? PlayersChanged;
    public event EventHandler? TurnStarted;
    public event EventHandler? TurnEnded;
    /// <summary>
    /// Occurs when any player wins the game.
    /// </summary>
    public event EventHandler<WinResult> PlayerWon;
    
    bool GameRunning { get; }
    
    /// <summary>
    ///     Gets current player.
    /// </summary>
    IPlayer CurrentPlayerInfo { get; }

    IPlayer WhitePlayer { get; }

    IPlayer BlackPlayer { get; }
    
    Figure[] Board { get; }
    
    /// <summary>
    ///     Set current players.
    /// </summary>
    void StartGame(IPlayer player1, IPlayer player2, PlayerColor startingPlayerColor, Figure[] board);

    /// <summary>
    ///     Sets next player as <see cref="CurrentPlayerInfo" />.
    /// </summary>
    void StartTurn();

    void EndTurn(TimeSpan? forcedTime = null);

    void Surrender();

    void StopGame();
    
    void PlayerLost(IPlayer player, WinType winType, bool publishResult);
    
    void PlayerWin(IPlayer player, WinType winType, bool publishResult);
}