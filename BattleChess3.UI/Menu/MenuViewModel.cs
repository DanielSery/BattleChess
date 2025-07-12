using BattleChess3.UI.Editor;
using BattleChess3.UI.Game;
using BattleChess3.UI.Shared;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Menu;

public class MenuViewModel : ViewModelBase
{
    private readonly MapsViewModel _mapsViewModel;
    private readonly BoardViewModel _boardViewModel;

    public MenuViewModel(
        BoardViewModel boardViewModel,
        MapsViewModel mapsViewModel,
        TeamBoardViewModel teamBoardViewModel,
        SignUpViewModel signUpViewModel,
        LoginViewModel loginViewModel,
        MultiplayerViewModel multiplayerViewModel,
        LeaderboardViewModel leaderboardViewModel)
    {
        _boardViewModel = boardViewModel;
        _mapsViewModel = mapsViewModel;
        TeamBoardViewModel = teamBoardViewModel;
        SignUpViewModel = signUpViewModel;
        LoginViewModel = loginViewModel;
        MultiplayerViewModel = multiplayerViewModel;
        LeaderboardViewModel = leaderboardViewModel;
        
        LocalGameCommand = new RelayCommand(LocalGame);
        SelectEditorCommand = new RelayCommand(SelectEditor);
        ShowLobbiesCommand = new RelayCommand(ShowLobbies);
        ShowLoginCommand = new RelayCommand(ShowLogin);
        ShowSignUpCommand = new RelayCommand(ShowSignUp);
        ShowLeaderboardCommand = new RelayCommand(ShowLeaderboard);
        
        SignUpViewModel.RequestEndSignUp += HideSideMenu;
        LoginViewModel.RequestEndLogin += HideSideMenu;
        LeaderboardViewModel.RequestEnd += HideSideMenu;
        MultiplayerViewModel.RequestEnd += HideSideMenu;
    }

    public LoginViewModel LoginViewModel { get; }
    public SignUpViewModel SignUpViewModel { get; }
    public TeamBoardViewModel TeamBoardViewModel { get; }
    public MultiplayerViewModel MultiplayerViewModel { get; }
    public LeaderboardViewModel LeaderboardViewModel { get; }

    private SelectedMenuTab _selectedMenuTab = SelectedMenuTab.None;
    public SelectedMenuTab SelectedMenuTab
    {
        get => _selectedMenuTab;
        set
        {
            var previousTab = _selectedMenuTab;
            if (!SetProperty(ref _selectedMenuTab, value)) 
                return;
            
            MultiplayerViewModel.OnDeactivation();
            LeaderboardViewModel.OnDeactivation();
            SignUpViewModel.OnDeactivation();
            SelectedTabChanged?.Invoke(this, (previousTab, value));
        }
    }

    public RelayCommand LocalGameCommand { get; }
    public RelayCommand SelectEditorCommand { get; }
    
    public RelayCommand ShowSignUpCommand { get; }
    public RelayCommand ShowLoginCommand { get; }
    public RelayCommand ShowLobbiesCommand { get; }
    public RelayCommand ShowLeaderboardCommand { get; }

    public event EventHandler? RequestSwitchToGame;
    public event EventHandler? RequestSwitchToEditor;
    public event EventHandler<(SelectedMenuTab, SelectedMenuTab)>? SelectedTabChanged; 
    
    private void LocalGame()
    {
        _boardViewModel.SinglePlayerLoadMap(_mapsViewModel.TeamMap);
        RequestSwitchToGame?.Invoke(this, EventArgs.Empty);
    }

    private void SelectEditor()
    {
        RequestSwitchToEditor?.Invoke(this, EventArgs.Empty);
    }

    private void ShowLobbies()
    {
        SelectedMenuTab = SelectedMenuTab.Lobby;
        MultiplayerViewModel.OnActivation();
    }

    private void ShowLogin()
    {
        SelectedMenuTab = SelectedMenuTab.Login;
    }

    private void ShowSignUp()
    {
        SelectedMenuTab = SelectedMenuTab.SignUp;
        SignUpViewModel.OnActivation();
    }

    private void ShowLeaderboard()
    {
        SelectedMenuTab = SelectedMenuTab.Leaderboard;
        LeaderboardViewModel.OnActivation();
    }

    private void HideSideMenu(object? sender, EventArgs e)
    {
        SelectedMenuTab = SelectedMenuTab.None;
    }
}