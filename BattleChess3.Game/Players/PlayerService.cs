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
    public event EventHandler<int>? PlayerWon;
    
    public Player CurrentPlayer => GetPlayer(_currentPlayerId);

    public Player GetPlayer(int id)
    {
        if (_players.TryGetValue(id, out var player))
        {
            return player;
        }

        _players[id] = new Player(id);
        return _players[id];
    }

    public void InitializePlayers(in int currentPlayerId, in bool multiplayer)
    {
        _players.Clear();
        _currentPlayerId = currentPlayerId;
        _isMultiplayer = multiplayer;
        CanMove = currentPlayerId == 1 || !_isMultiplayer;
        IsWaitingForMove = currentPlayerId == 2 && _isMultiplayer;
    }

    public void NextTurn()
    {
        _currentPlayerId = _currentPlayerId == 1 ? 2 : 1;
        CanMove = CurrentPlayer.Id == 1 || !_isMultiplayer;
        IsWaitingForMove = CurrentPlayer.Id == 2 && _isMultiplayer;

        foreach (var player in _players)
        {
            if (player.Value.Equals(Player.Neutral))
                continue;
            
            if (player.Value.Figures.Count == 0)
                continue;

            if (!player.Value.Figures.Any(x => x.IsKing))
            {
                PlayerWon?.Invoke(this, 3 - player.Value.Id);
                CanMove = false;
                IsWaitingForMove = false;
            }
        }
    }
}