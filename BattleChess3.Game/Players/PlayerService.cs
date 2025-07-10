namespace BattleChess3.Game.Players;

internal class PlayerService : IPlayerService
{
    private readonly IDictionary<int, Player> _players = new Dictionary<int, Player>();
    private int _currentPlayerId;
    private bool _isMultiplayer;

    public PlayerService()
    {
        _players.Clear();
        _currentPlayerId = 0;
    }
    
    public bool CanMove { get; private set; }
    
    public bool IsWaitingForMove { get; private set; }

    /// <inheritdoc />
    public TimeSpan TimeSpent { get; } = TimeSpan.Zero;

    public bool HasTimer { get; private set; }

    /// <inheritdoc />
    public event EventHandler<(Player won, Player lost)>? PlayerWon;
    
    public Player CurrentPlayer => GetPlayer(_currentPlayerId);

    public Player GetPlayer(int id)
    {
        return _players[id];
    }

    public void InitializePlayers(Player player1, Player player2, int currentPlayerId, bool multiplayer, bool hasTimer)
    {
        _players.Clear();
        _players[0] = Player.Neutral;
        _players[1] = player1;
        _players[2] = player2;
        
        _currentPlayerId = currentPlayerId;
        _isMultiplayer = multiplayer;
        HasTimer = hasTimer;
        CanMove = currentPlayerId == 1 || !_isMultiplayer;
        IsWaitingForMove = currentPlayerId == 2 && _isMultiplayer;
    }

    public void NextTurn()
    {
        _currentPlayerId = _currentPlayerId == 1 ? 2 : 1;
        CanMove = CurrentPlayer.Index == 1 || !_isMultiplayer;
        IsWaitingForMove = CurrentPlayer.Index == 2 && _isMultiplayer;
        
        EvaluateLost(_players[1], _players[2]);
        EvaluateLost(_players[2], _players[1]);
    }

    private void EvaluateLost(Player evaluatedPlayer, Player otherPlayer)
    {
        if (evaluatedPlayer.Figures.Any(x => x.IsKing))
            return;
        
        PlayerWon?.Invoke(this, (otherPlayer, evaluatedPlayer));
        CanMove = false;
        IsWaitingForMove = false;
    }

    /// <inheritdoc />
    public void StopTimer()
    {
    }
}