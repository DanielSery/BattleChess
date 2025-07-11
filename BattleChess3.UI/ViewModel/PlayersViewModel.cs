using BattleChess3.Game.Players;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class PlayersViewModel : ViewModelBase
{
    private readonly IPlayerService _playerService;

    public PlayersViewModel(IPlayerService playerService)
    {
        _playerService = playerService;
        _playerService.PlayersChanged += PlayerServiceOnPlayersChanged;
        _playerService.TurnStarted += PlayerServiceOnTurnStarted;
        _playerService.TurnEnded += PlayerServiceOnTurnEnded;
        _playerService.PlayerWon += PlayerServiceOnPlayerWon;
    }

    private PlayerViewModel[] _players = [];
    public PlayerViewModel[] Players
    {
        get => _players;
        set => SetProperty(ref _players, value);
    }
    
    private bool _canExit = true;
    public bool CanExit
    {
        get => _canExit;
        set => SetProperty(ref _canExit, value);
    }

    private bool _hasTimer;
    public bool HasTimer
    {
        get => _hasTimer;
        set => SetProperty(ref _hasTimer, value);
    }

    private void PlayerServiceOnPlayersChanged(object? sender, EventArgs e)
    {
        var players = _playerService.GetPlayers();
        Players = players
            .Where(x => !x.Equals(Player.Neutral))
            .Select(x => new PlayerViewModel(x, _playerService.HasTimer))
            .ToArray();

        CanExit = !_playerService.IsMultiplayer;
        HasTimer = _playerService.HasTimer;
    }

    private void PlayerServiceOnTurnStarted(object? sender, EventArgs e)
    {
        var currentPlayer = Players.FirstOrDefault(x => x.Index == _playerService.CurrentPlayer.Index);
        currentPlayer?.StartTurn(_playerService.CurrentPlayer.RemainingTime);
    }

    private void PlayerServiceOnTurnEnded(object? sender, EventArgs e)
    {
        var currentPlayer = Players.FirstOrDefault(x => x.Index == _playerService.CurrentPlayer.Index);
        currentPlayer?.EndTurn();
    }

    private void PlayerServiceOnPlayerWon(object? sender, (Player? won, Player? lost) e)
    {
        CanExit = true;
    }
}