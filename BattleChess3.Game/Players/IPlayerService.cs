namespace BattleChess3.Game.Players;

public interface IPlayerService
{
    /// <summary>
    /// Occurs when any player wins the game.
    /// </summary>
    public event EventHandler<int> PlayerWon;
    
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
    void InitializePlayers(in int currentPlayer);

    /// <summary>
    ///     Sets next player as <see cref="CurrentPlayer" />.
    /// </summary>
    void NextTurn();
}