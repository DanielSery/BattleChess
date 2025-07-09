using BattleChess3.Multiplayer;
using BattleChess3.Multiplayer.Tables;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class LeaderboardViewModel : ViewModelBase
{
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;

    public LeaderboardViewModel( 
        IMultiplayerPlayerService multiplayerPlayerService)
    {
        _multiplayerPlayerService = multiplayerPlayerService;
    }
    
    private List<PublicPlayerData> _leaderboard = new List<PublicPlayerData>();
    public List<PublicPlayerData> Leaderboard
    {
        get => _leaderboard;
        set => SetProperty(ref _leaderboard, value);
    }

    public void OnActivation()
    {
        _multiplayerPlayerService.GetLeaderboard()
            .ContinueWith(x =>
            {
                Leaderboard = x.Result;
            });
    }

    public void OnDeactivation()
    {
    }
}