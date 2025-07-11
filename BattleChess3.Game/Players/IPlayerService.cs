namespace BattleChess3.Game.Players;

public interface IPlayerService
{
    public event EventHandler? PlayersChanged;
    public event EventHandler? TurnStarted;
    public event EventHandler? TurnEnded;
    /// <summary>
    /// Occurs when any player wins the game.
    /// </summary>
    public event EventHandler<(bool notifyOther, WinType winType, Player? won, Player? lost)> PlayerWon;
    
    bool CanMove { get; }
    
    bool IsWaitingForMove { get; }
    
    /// <summary>
    ///     Gets current player.
    /// </summary>
    Player CurrentPlayer { get; }

    bool IsMultiplayer { get; }
    bool HasTimer { get; }

    /// <summary>
    ///     Gets player with id.
    /// </summary>
    Player GetPlayer(int id);

    Player[] GetPlayers();

    /// <summary>
    ///     Set current players.
    /// </summary>
    void InitializePlayers(Player player1, Player player2, int currentPlayerId, bool multiplayer, bool hasTimer);

    TimeSpan EndTurn(TimeSpan? forcedTime = null);

    /// <summary>
    ///     Sets next player as <see cref="CurrentPlayer" />.
    /// </summary>
    void NextTurn();

    void Surrender();
    
    void PlayerLost(Player player, WinType winType, bool notifyOther);
    
    void PlayerWin(Player player, WinType winType, bool notifyOther);
}