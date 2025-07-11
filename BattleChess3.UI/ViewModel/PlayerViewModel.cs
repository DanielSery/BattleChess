using System.Windows.Threading;
using BattleChess3.Game.Players;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class PlayerViewModel : ViewModelBase
{
    private readonly bool _hasTimer;
    private readonly Player _player;
    private readonly DispatcherTimer _timer;
    private readonly TimeSpan _initialIdleTime = TimeSpan.FromSeconds(60);
    
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

    public PlayerViewModel(Player player, bool hasTimer)
    {
        _player = player;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(0.05)
        };
        _timer.Tick += TimerTick;

        _hasTimer = hasTimer;
        UpdateTimerText();
    }


    public void StartTurn(TimeSpan initialTime)
    {
        IsHisTurn = true;
        if (!_timer.IsEnabled && _hasTimer)
        {
            _timer.Start();
        }
    }

    public void EndTurn()
    {
        IsHisTurn = false;
        if (_hasTimer)
        {
            _timer.Stop();
            IdleTime = _initialIdleTime.ToString(@"mm\:ss");
        }
    }
    
    private void TimerTick(object? sender, EventArgs e)
    {
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (!_hasTimer)
            return;
        
        var timeRemaining = _player.RemainingTime - _player.CurrentStopwatch.Elapsed;
        var idleTimeRemaining = _initialIdleTime - _player.CurrentStopwatch.Elapsed;
        
        if (timeRemaining.TotalSeconds > 0 && idleTimeRemaining.TotalSeconds > 0)
        {
            timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));
            idleTimeRemaining = idleTimeRemaining.Subtract(TimeSpan.FromSeconds(1));
            RemainingTime = timeRemaining.ToString(@"m\:ss\.f");
            IdleTime = idleTimeRemaining.ToString(@"m\:ss\.f");
        }
        else
        {
            _timer.Stop();
            RemainingTime = "00:00";
        }
    }
}