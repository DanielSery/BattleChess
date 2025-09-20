using System.Diagnostics;
using System.Windows.Threading;
using CrownsGuard.Game;
using CrownsGuard.Game.Players;
using CrownsGuard.Game.Timers;
using CrownsGuard.Multiplayer.Game;
using CrownsGuard.Multiplayer.Players;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Multiplayer;

public class PlayerViewModel : ViewModelBase
{
    private readonly IPlayer _playerInfo;
    private readonly IGameService _gameService;
    private readonly DispatcherTimer _timer;
    private readonly Stopwatch  _stopwatch;
    
    public PlayerColor PlayerColor => _playerInfo.PlayerColor;
    public string FullName { get; }

    private bool _isHisTurn;
    public bool IsHisTurn
    {
        get => _isHisTurn;
        set => SetProperty(ref _isHisTurn, value);
    }

    private string _remainingTime = string.Empty;
    public string RemainingTime
    {
        get => _remainingTime;
        set
        {
            _remainingTime = value;
            OnPropertyChanged();
        }
    }

    private string _idleTime = string.Empty;
    public string IdleTime
    {
        get => _idleTime;
        set
        {
            _idleTime = value;
            OnPropertyChanged();
        }
    }

    public PlayerViewModel(IPlayer player, IGameService gameService)
    {
        _stopwatch = new Stopwatch();
        if (player is IOnlinePlayerInfo { Elo: not null } onlinePlayerInfo)
        {
            FullName = $"{onlinePlayerInfo.Name} ({onlinePlayerInfo.Elo})";
        }
        else
        {
            FullName = player.Name;
        }
        
        _playerInfo = player;
        _gameService = gameService;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(0.05)
        };
        _timer.Tick += TimerTick;

        UpdateTimerText();
    }


    public void StartTurn()
    {
        _stopwatch.Restart();
        IsHisTurn = true;
        if (!_timer.IsEnabled)
        {
            _timer.Start();
        }
    }

    public void EndTurn()
    {
        _stopwatch.Stop();
        IsHisTurn = false;
        _timer.Stop();
        IdleTime = IMultiplayerGameService.TurnTimeout.ToString(@"mm\:ss");
    }

    public void StopTimers()
    {
        IsHisTurn = false;
        _timer.Stop();
    }
    
    private void TimerTick(object? sender, EventArgs e)
    {
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        var isMultiplayer = _gameService.BlackPlayer is IOnlinePlayerInfo;
        var hasTimer = _gameService.WhitePlayer.Timer is not InfinitePlayerTimer;

        if (isMultiplayer && !hasTimer)
        {
            var idleTimeRemaining = IMultiplayerGameService.TurnTimeout - _stopwatch.Elapsed;
            if (idleTimeRemaining.TotalSeconds > 0)
            {
                IdleTime = idleTimeRemaining.ToString(@"m\:ss\.f");
            }
            else
            {
                _timer.Stop();
                IdleTime = "00:00";
                if (_playerInfo.PlayerColor == PlayerColor.White)
                {
                    _gameService.PlayerLost(_playerInfo, WinType.NotResponding, false);
                }
            }
        }
        else if (isMultiplayer && hasTimer)
        {
            var timeRemaining = _playerInfo.Timer.RemainingTime - _stopwatch.Elapsed;
            var idleTimeRemaining = IMultiplayerGameService.TurnTimeout - _stopwatch.Elapsed;
        
            if (timeRemaining.TotalSeconds > 0 && idleTimeRemaining.TotalSeconds > 0)
            {
                RemainingTime = timeRemaining.ToString(@"m\:ss\.f");
                IdleTime = idleTimeRemaining.ToString(@"m\:ss\.f");
            }
            else if (timeRemaining.TotalSeconds <= 0)
            {
                _timer.Stop();
                RemainingTime = "0:00";
                _gameService.PlayerLost(_playerInfo, WinType.OutOfTime, _playerInfo.PlayerColor == PlayerColor.White);
            }
            else if (idleTimeRemaining.TotalMinutes <= 0)
            {
                _timer.Stop();
                IdleTime = "0:00";
                if (_playerInfo.PlayerColor == PlayerColor.White)
                {
                    _gameService.PlayerLost(_playerInfo, WinType.NotResponding, false);
                }
            }
        }
    }
}