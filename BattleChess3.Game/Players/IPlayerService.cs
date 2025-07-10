namespace BattleChess3.Game.Players;

public interface IPlayerService
{
    /// <summary>
    /// Occurs when any player wins the game.
    /// </summary>
    public event EventHandler<(Player won, Player lost)> PlayerWon;
    
    bool CanMove { get; }
    
    bool IsWaitingForMove { get; }
    TimeSpan TimeSpent { get; }
    
    /// <summary>
    ///     Gets current player.
    /// </summary>
    Player CurrentPlayer { get; }

    /// <summary>
    ///     Gets player with id.
    /// </summary>
    Player GetPlayer(int id);

    /// <summary>
    ///     Set current players.
    /// </summary>
    void InitializePlayers(Player player1, Player player2, int currentPlayerId, bool multiplayer, bool hasTimer);

    /// <summary>
    ///     Sets next player as <see cref="CurrentPlayer" />.
    /// </summary>
    void NextTurn();

    void StopTimer();
}