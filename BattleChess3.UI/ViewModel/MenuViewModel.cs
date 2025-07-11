
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

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

    private bool _lobbyShown;
    public bool LobbyShown
    {
        get => _lobbyShown;
        set => SetTabSelected(out _lobbyShown, value);
    }

    private bool _loginShown;
    public bool LoginShown
    {
        get => _loginShown;
        set => SetTabSelected(out _loginShown, value);
    }

    private bool _signUpShown;
    public bool SignUpShown
    {
        get => _signUpShown;
        set => SetTabSelected(out _signUpShown, value);
    }

    private bool _leaderboardShown;
    public bool LeaderboardShown
    {
        get => _leaderboardShown;
        set => SetTabSelected(out _leaderboardShown, value);
    }

    public RelayCommand LocalGameCommand { get; }
    public RelayCommand SelectEditorCommand { get; }
    
    public RelayCommand ShowSignUpCommand { get; }
    public RelayCommand ShowLoginCommand { get; }
    public RelayCommand ShowLobbiesCommand { get; }
    public RelayCommand ShowLeaderboardCommand { get; }

    public event EventHandler? RequestSwitchToGame;
    public event EventHandler? RequestSwitchToEditor;
    
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
        LobbyShown = true;
        MultiplayerViewModel.OnActivation();
    }

    private void ShowLogin()
    {
        LoginShown = true;
    }

    private void ShowSignUp()
    {
        SignUpShown = true;
        SignUpViewModel.OnActivation();
    }

    private void ShowLeaderboard()
    {
        LeaderboardShown = true;
        LeaderboardViewModel.OnActivation();
    }

    private void HideSideMenu(object? sender, EventArgs e)
    {
        SetTabSelected(out _, false); 
    }

    private void SetTabSelected(out bool selectedTab, bool value)
    {
        MultiplayerViewModel.OnDeactivation();
        LeaderboardViewModel.OnDeactivation();
        SignUpViewModel.OnDeactivation();
        
        _loginShown = false;
        _signUpShown = false;
        _lobbyShown = false;
        _leaderboardShown = false;
        selectedTab = value;

        RaisePropertyChanged(nameof(LoginShown));
        RaisePropertyChanged(nameof(SignUpShown));
        RaisePropertyChanged(nameof(LobbyShown));
        RaisePropertyChanged(nameof(LeaderboardShown));
    }
}