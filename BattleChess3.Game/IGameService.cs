using BattleChess3.Game.Players;

namespace BattleChess3.Game;

public interface IGameService
{
    public event EventHandler? PlayersChanged;
    public event EventHandler? TurnStarted;
    public event EventHandler? TurnEnded;
    /// <summary>
    /// Occurs when any player wins the game.
    /// </summary>
    public event EventHandler<(bool notifyOther, WinType winType, PlayerInfo? won, PlayerInfo? lost)> PlayerWon;
    
    bool CanMove { get; }
    
    bool IsWaitingForMove { get; }
    
    /// <summary>
    ///     Gets current player.
    /// </summary>
    PlayerInfo CurrentPlayerInfo { get; }

    bool IsMultiplayer { get; }
    bool HasTimer { get; }

    /// <summary>
    ///     Gets player with id.
    /// </summary>
    PlayerInfo GetPlayerInfo(Player player);

    PlayerInfo[] GetPlayerInfos();

    /// <summary>
    ///     Set current players.
    /// </summary>
    void StartGame(PlayerInfo player1, PlayerInfo player2, Player currentPlayer, bool multiplayer, bool hasTimer);

    TimeSpan EndTurn(TimeSpan? forcedTime = null);

    /// <summary>
    ///     Sets next player as <see cref="CurrentPlayerInfo" />.
    /// </summary>
    void NextTurn();

    void Surrender();
    
    void PlayerLost(PlayerInfo player, WinType winType, bool notifyOther);
    
    void PlayerWin(PlayerInfo player, WinType winType, bool notifyOther);
}