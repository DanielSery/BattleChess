using System.Windows.Threading;
using CrownsGuard.Core.Players;
using CrownsGuard.Game;
using CrownsGuard.Game.Players;
using CrownsGuard.Game.Timers;
using CrownsGuard.Multiplayer.Game;
using CrownsGuard.Multiplayer.Players;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Multiplayer;

public class PlayerViewModel : ViewModelBase
{
    private readonly IPlayerInfo _playerInfo;
    private readonly IGameService _gameService;
    private readonly DispatcherTimer _timer;
    
    public Player Player => _playerInfo.Player;
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

    public PlayerViewModel(IPlayerInfo player, IGameService gameService)
    {
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


    public void StartTurn(TimeSpan initialTime)
    {
        IsHisTurn = true;
        if (!_timer.IsEnabled)
        {
            _timer.Start();
        }
    }

    public void EndTurn()
    {
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
            var idleTimeRemaining = IMultiplayerGameService.TurnTimeout - _playerInfo.Timer.LastTurnElapsedTime;
            if (idleTimeRemaining.TotalSeconds > 0)
            {
                IdleTime = idleTimeRemaining.ToString(@"m\:ss\.f");
            }
            else
            {
                _timer.Stop();
                IdleTime = "00:00";
                if (_playerInfo.Player == Player.White)
                {
                    _gameService.PlayerLost(_playerInfo, WinType.NotResponding, false);
                }
            }
        }
        else if (isMultiplayer && hasTimer)
        {
            var timeRemaining = _playerInfo.Timer.RemainingTime - _playerInfo.Timer.LastTurnElapsedTime;
            var idleTimeRemaining = IMultiplayerGameService.TurnTimeout - _playerInfo.Timer.LastTurnElapsedTime;
        
            if (timeRemaining.TotalSeconds > 0 && idleTimeRemaining.TotalSeconds > 0)
            {
                RemainingTime = timeRemaining.ToString(@"m\:ss\.f");
                IdleTime = idleTimeRemaining.ToString(@"m\:ss\.f");
            }
            else if (timeRemaining.TotalSeconds <= 0)
            {
                _timer.Stop();
                RemainingTime = "0:00";
                _gameService.PlayerLost(_playerInfo, WinType.OutOfTime, _playerInfo.Player == Player.White);
            }
            else if (idleTimeRemaining.TotalMinutes <= 0)
            {
                _timer.Stop();
                IdleTime = "0:00";
                if (_playerInfo.Player == Player.White)
                {
                    _gameService.PlayerLost(_playerInfo, WinType.NotResponding, false);
                }
            }
        }
    }
}