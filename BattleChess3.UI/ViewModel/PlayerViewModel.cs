using System.Windows.Threading;
using BattleChess3.Game.Players;
using BattleChess3.Multiplayer;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class PlayerViewModel : ViewModelBase
{
    private readonly Player _player;
    private readonly IPlayerService _playerService;
    private readonly DispatcherTimer _timer;
    
    public int Index => _player.Index;
    public string FullName => _player.Elo is null ? _player.Name : $"{_player.Name} ({_player.Elo})";
    public string Name => _player.Name;
    public int? Elo => _player.Elo;

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

    public PlayerViewModel(Player player, IPlayerService playerService)
    {
        _player = player;
        _playerService = playerService;

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
        if (_playerService is { IsMultiplayer: true, HasTimer: false })
        {
            var idleTimeRemaining = IMultiplayerGameService.TurnTimeout - _player.CurrentStopwatch.Elapsed;
            if (idleTimeRemaining.TotalSeconds > 0)
            {
                IdleTime = idleTimeRemaining.ToString(@"m\:ss\.f");
            }
            else
            {
                _timer.Stop();
                IdleTime = "00:00";
                if (_player.Index == 1)
                {
                    _playerService.PlayerLost(_player, WinType.NotResponding, false);
                }
            }
        }
        else if (_playerService is { IsMultiplayer: true, HasTimer: true })
        {
            var timeRemaining = _player.RemainingTime - _player.CurrentStopwatch.Elapsed;
            var idleTimeRemaining = IMultiplayerGameService.TurnTimeout - _player.CurrentStopwatch.Elapsed;
        
            if (timeRemaining.TotalSeconds > 0 && idleTimeRemaining.TotalSeconds > 0)
            {
                RemainingTime = timeRemaining.ToString(@"m\:ss\.f");
                IdleTime = idleTimeRemaining.ToString(@"m\:ss\.f");
            }
            else if (timeRemaining.TotalSeconds <= 0)
            {
                _timer.Stop();
                RemainingTime = "0:00";
                _playerService.PlayerLost(_player, WinType.OutOfTime, _player.Index == 1);
            }
            else if (idleTimeRemaining.TotalMinutes <= 0)
            {
                _timer.Stop();
                IdleTime = "0:00";
                if (_player.Index == 1)
                {
                    _playerService.PlayerLost(_player, WinType.NotResponding, false);
                }
            }
        }
    }
}