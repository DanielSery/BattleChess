using BattleChess3.Game;
using BattleChess3.Game.Players;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Multiplayer;

public class GameViewModel : ViewModelBase
{
    private readonly IGameService _gameService;

    public GameViewModel(IGameService gameService)
    {
        _gameService = gameService;
        _gameService.PlayersChanged += GameServiceOnGamesChanged;
        _gameService.TurnStarted += GameServiceOnTurnStarted;
        _gameService.TurnEnded += GameServiceOnTurnEnded;
        _gameService.PlayerWon += GameServiceOnGameWon;
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
            if (!_gameService.IsMultiplayer)
            {
                return true;
            }

            return !_gameService.IsWaitingForMove;
        }
    }

    private void GameServiceOnGamesChanged(object? sender, EventArgs e)
    {
        Players = _gameService.GetPlayerInfos()
            .Where(x => !x.Equals(PlayerInfo.Neutral))
            .Select(x => new PlayerViewModel(x, _gameService))
            .ToArray();

        CanExit = !_gameService.IsMultiplayer;
        RaisePropertyChanged(nameof(CanEndGame));
        IsRanked = _gameService is { IsMultiplayer: true, HasTimer: true };
        IsLobby = _gameService is { IsMultiplayer: true, HasTimer: false };
        IsLocalGame = !_gameService.IsMultiplayer;
    }

    private void GameServiceOnTurnStarted(object? sender, EventArgs e)
    {
        var currentPlayer = Players.FirstOrDefault(x => x.Player == _gameService.CurrentPlayerInfo.Player);
        currentPlayer?.StartTurn(_gameService.CurrentPlayerInfo.RemainingTime);
        RaisePropertyChanged(nameof(CanEndGame));
    }

    private void GameServiceOnTurnEnded(object? sender, EventArgs e)
    {
        var currentPlayer = Players.FirstOrDefault(x => x.Player == _gameService.CurrentPlayerInfo.Player);
        currentPlayer?.EndTurn();
        RaisePropertyChanged(nameof(CanEndGame));
    }

    private void GameServiceOnGameWon(object? sender, (bool notifyOther, WinType winType, PlayerInfo? won, PlayerInfo? lost) e)
    {
        CanExit = true;
        RaisePropertyChanged(nameof(CanEndGame));
        foreach (var player in Players)
        {
            player.StopTimers();
        }
    }
}