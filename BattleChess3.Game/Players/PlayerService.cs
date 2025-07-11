namespace BattleChess3.Game.Players;

internal class PlayerService : IPlayerService
{
    private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();
    private int _currentPlayerId;
    
    public bool CanMove { get; private set; }
    
    public bool IsWaitingForMove { get; private set; }

    public bool HasTimer { get; private set; }
    
    public bool IsMultiplayer { get; private set; }

    public event EventHandler? PlayersChanged;
    public event EventHandler? TurnStarted;
    public event EventHandler? TurnEnded;

    /// <inheritdoc />
    public event EventHandler<(bool notifyOther, WinType winType, Player? won, Player? lost)>? PlayerWon;
    
    public Player CurrentPlayer => GetPlayer(_currentPlayerId);

    public Player GetPlayer(int id)
    {
        return _players[id];
    }

    public Player[] GetPlayers()
    {
        return _players.Values.ToArray();
    }

    public void InitializePlayers(Player player1, Player player2, int currentPlayerId, bool multiplayer, bool hasTimer)
    {
        _players.Clear();
        _players[0] = Player.Neutral;
        _players[1] = player1;
        _players[2] = player2;
        
        _currentPlayerId = currentPlayerId;
        IsMultiplayer = multiplayer;
        HasTimer = hasTimer;
        CanMove = currentPlayerId == 1 || !IsMultiplayer;
        IsWaitingForMove = currentPlayerId == 2 && IsMultiplayer;
        
        PlayersChanged?.Invoke(this, EventArgs.Empty);
        StartTurn();
    }

    public void NextTurn()
    {
        _currentPlayerId = _currentPlayerId == 1 ? 2 : 1;
        CanMove = _currentPlayerId == 1 || !IsMultiplayer;
        IsWaitingForMove = _currentPlayerId == 2 && IsMultiplayer;
        
        EvaluateLost(_players[1], _players[2]);
        EvaluateLost(_players[2], _players[1]);

        if (CanMove || IsWaitingForMove)
        {
            StartTurn();
        }
    }

    public void Surrender()
    {
        if (CanMove || IsWaitingForMove)
        {
            CanMove = false;
            IsWaitingForMove = false;
            PlayerWon?.Invoke(this, IsMultiplayer 
                ? (true, WinType.Surrender, _players[2], _players[1]) 
                : (false, WinType.Surrender, null, null));
        }
    }

    /// <inheritdoc />
    public void PlayerLost(Player player, WinType winType, bool notifyOther)
    {
        CanMove = false;
        IsWaitingForMove = false;
        PlayerWon?.Invoke(this, player.Index == 1 
            ? (notifyOther, winType, _players[2], _players[1]) 
            : (notifyOther, winType, _players[1], _players[2]));
    }

    /// <inheritdoc />
    public void PlayerWin(Player player, WinType winType, bool notifyOther)
    {
        CanMove = false;
        IsWaitingForMove = false;
        PlayerWon?.Invoke(this, player.Index == 1 
            ? (notifyOther, winType, _players[1], _players[2]) 
            : (notifyOther, winType, _players[2], _players[1]));
    }

    private void EvaluateLost(Player evaluatedPlayer, Player otherPlayer)
    {
        if (evaluatedPlayer.Figures.Any(x => x.IsKing))
            return;
        
        CanMove = false;
        IsWaitingForMove = false;
        PlayerWon?.Invoke(this, (otherPlayer.Index == 1, WinType.CapturedKing, otherPlayer, evaluatedPlayer));
    }

    private void StartTurn()
    {
        CurrentPlayer.StartTurn();
        TurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public TimeSpan EndTurn(TimeSpan? forcedTime = null)
    {
        var timeSpent = CurrentPlayer.OnEndingTurn(forcedTime);
        TurnEnded?.Invoke(this, EventArgs.Empty);
        return timeSpent;
    }
}