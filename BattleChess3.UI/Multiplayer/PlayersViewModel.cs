using BattleChess3.Game.Players;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Multiplayer;

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

    private bool _isLocalGame = true;
    public bool IsLocalGame
    {
        get => _isLocalGame;
        set => SetProperty(ref _isLocalGame, value);
    }

    private bool _isLobby;
    public bool IsLobby
    {
        get => _isLobby;
        set => SetProperty(ref _isLobby, value);
    }

    private bool _isRanked;
    public bool IsRanked
    {
        get => _isRanked;
        set => SetProperty(ref _isRanked, value);
    }
    
    public bool CanEndGame
    {
        get
        {
            if (!_playerService.IsMultiplayer)
            {
                return true;
            }

            return !_playerService.IsWaitingForMove;
        }
    }

    private void PlayerServiceOnPlayersChanged(object? sender, EventArgs e)
    {
        var players = _playerService.GetPlayers();
        Players = players
            .Where(x => !x.Equals(Player.Neutral))
            .Select(x => new PlayerViewModel(x, _playerService))
            .ToArray();

        CanExit = !_playerService.IsMultiplayer;
        RaisePropertyChanged(nameof(CanEndGame));
        IsRanked = _playerService is { IsMultiplayer: true, HasTimer: true };
        IsLobby = _playerService is { IsMultiplayer: true, HasTimer: false };
        IsLocalGame = !_playerService.IsMultiplayer;
    }

    private void PlayerServiceOnTurnStarted(object? sender, EventArgs e)
    {
        var currentPlayer = Players.FirstOrDefault(x => x.Index == _playerService.CurrentPlayer.Index);
        currentPlayer?.StartTurn(_playerService.CurrentPlayer.RemainingTime);
        RaisePropertyChanged(nameof(CanEndGame));
    }

    private void PlayerServiceOnTurnEnded(object? sender, EventArgs e)
    {
        var currentPlayer = Players.FirstOrDefault(x => x.Index == _playerService.CurrentPlayer.Index);
        currentPlayer?.EndTurn();
        RaisePropertyChanged(nameof(CanEndGame));
    }

    private void PlayerServiceOnPlayerWon(object? sender, (bool notifyOther, WinType winType, Player? won, Player? lost) e)
    {
        CanExit = true;
        RaisePropertyChanged(nameof(CanEndGame));
        foreach (var player in Players)
        {
            player.StopTimers();
        }
    }
}