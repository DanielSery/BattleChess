using CrownsGuard.Multiplayer;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Tables;
using CommunityToolkit.Mvvm.Input;
using CrownsGuard.UI.Services;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Multiplayer;

public class LeaderboardViewModel : ViewModelBase
{
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    private readonly ILoadingService _loadingService;

    public LeaderboardViewModel( 
        IMultiplayerPlayerService multiplayerPlayerService,
        ILoadingService loadingService)
    {
        _multiplayerPlayerService = multiplayerPlayerService;
        _loadingService = loadingService;

        RequestEndCommand = new RelayCommand(RaiseRequestEnd);
    }


    private List<PublicPlayerData> _leaderboard = new List<PublicPlayerData>();
    public List<PublicPlayerData> Leaderboard
    {
        get => _leaderboard;
        set => SetProperty(ref _leaderboard, value);
    }
    
    public event EventHandler? RequestEnd;
    
    public RelayCommand RequestEndCommand { get; }

    public void OnActivation()
    {
        Task.Run(async () =>
        {
            using var loading = _loadingService.StartLoadingOperation("Getting leaderboard");
            Leaderboard = await _multiplayerPlayerService.GetLeaderboard(loading.CancellationToken);
        });
    }

    public void OnDeactivation()
    {
    }

    private void RaiseRequestEnd()
    {
        RequestEnd?.Invoke(this, EventArgs.Empty);
    }
}