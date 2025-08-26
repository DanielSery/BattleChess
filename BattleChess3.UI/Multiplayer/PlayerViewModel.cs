using System.Windows.Threading;
using BattleChess3.Game.Players;
using BattleChess3.Multiplayer;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Multiplayer;

public class PlayerViewModel : ViewModelBase
{
    private readonly PlayerInfo _playerInfo;
    private readonly IGameService _gameService;
    private readonly DispatcherTimer _timer;
    
    public Player Player => _playerInfo.Player;
    public string FullName => _playerInfo.Elo is null ? _playerInfo.Name : $"{_playerInfo.Name} ({_playerInfo.Elo})";
    public string Name => _playerInfo.Name;
    public int? Elo => _playerInfo.Elo;

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

    public PlayerViewModel(PlayerInfo player, IGameService gameService)
    {
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
        if (_gameService is { IsMultiplayer: true, HasTimer: false })
        {
            var idleTimeRemaining = IMultiplayerGameService.TurnTimeout - _playerInfo.CurrentStopwatch.Elapsed;
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
        else if (_gameService is { IsMultiplayer: true, HasTimer: true })
        {
            var timeRemaining = _playerInfo.RemainingTime - _playerInfo.CurrentStopwatch.Elapsed;
            var idleTimeRemaining = IMultiplayerGameService.TurnTimeout - _playerInfo.CurrentStopwatch.Elapsed;
        
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