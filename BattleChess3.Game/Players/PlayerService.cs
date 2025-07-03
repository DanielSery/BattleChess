namespace BattleChess3.Game.Players;

internal class PlayerService : IPlayerService
{
    private readonly IDictionary<int, Player> _players = new Dictionary<int, Player>();
    private int _currentPlayerId;

    public PlayerService()
    {
        _players.Clear();
        _currentPlayerId = 0;
    }

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

    public void InitializePlayers(in int currentPlayerId)
    {
        _players.Clear();
        _currentPlayerId = currentPlayerId;
    }

    public void NextTurn()
    {
        NextPlayer();

        foreach (var player in _players)
        {
            if (player.Value.Equals(Player.Neutral))
                continue;
            
            if (player.Value.Figures.Count == 0)
                continue;
            
            if (!player.Value.Figures.Any(x => x.IsKing))
                PlayerWon?.Invoke(this, 3 - player.Value.Id);
        }
    }

    private void NextPlayer()
    {
        _currentPlayerId = (_currentPlayerId + 1) % _players.Count;
        if (_currentPlayerId == 0)
        {
            _currentPlayerId = 1;
        }
    }
}