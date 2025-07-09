using BattleChess3.Multiplayer;
using BattleChess3.Multiplayer.Tables;
using BattleChess3.UI.Services;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

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
    }
    
    private List<PublicPlayerData> _leaderboard = new List<PublicPlayerData>();
    public List<PublicPlayerData> Leaderboard
    {
        get => _leaderboard;
        set => SetProperty(ref _leaderboard, value);
    }

    public void OnActivation()
    {
        Task.Run(async () =>
        {
            using var loading = _loadingService.StartLoadingOperation();
            Leaderboard = await _multiplayerPlayerService.GetLeaderboard(loading.CancellationToken);
        });
    }

    public void OnDeactivation()
    {
    }
}