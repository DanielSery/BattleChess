using System.Windows;
using CrownsGuard.UI.Game;
using CrownsGuard.UI.Multiplayer;
using CrownsGuard.UI.Services;
using CrownsGuard.UI.Settings;
using CrownsGuard.UI.Shared;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.MainWindow;

public class MenuViewModel : ViewModelBase
{
    private readonly ISoundService _soundService;
    private readonly MapsViewModel _mapsViewModel;
    private readonly BoardViewModel _boardViewModel;

    public MenuViewModel(
        ISoundService soundService,
        BoardViewModel boardViewModel,
        MapsViewModel mapsViewModel,
        SignUpViewModel signUpViewModel,
        LoginViewModel loginViewModel,
        MultiplayerViewModel multiplayerViewModel,
        LeaderboardViewModel leaderboardViewModel,
        SettingsViewModel settingsViewModel)
    {
        _soundService = soundService;
        _boardViewModel = boardViewModel;
        _mapsViewModel = mapsViewModel;
        SignUpViewModel = signUpViewModel;
        LoginViewModel = loginViewModel;
        MultiplayerViewModel = multiplayerViewModel;
        LeaderboardViewModel = leaderboardViewModel;
        SettingsViewModel = settingsViewModel;
        
        LocalGameCommand = new RelayCommand(LocalGame);
        SelectEditorCommand = new RelayCommand(SelectEditor);
        ShowLobbiesCommand = new RelayCommand(ShowLobbies);
        ShowLoginCommand = new RelayCommand(ShowLogin);
        ShowSignUpCommand = new RelayCommand(ShowSignUp);
        ShowLeaderboardCommand = new RelayCommand(ShowLeaderboard);
        ShowSettingsCommand = new RelayCommand(ShowSettings);
        ExitCommand = new RelayCommand(Exit);
        
        SignUpViewModel.RequestEndSignUp += HideSideMenu;
        LoginViewModel.RequestEndLogin += HideSideMenu;
        LeaderboardViewModel.RequestEnd += HideSideMenu;
        MultiplayerViewModel.RequestEnd += HideSideMenu;
        SettingsViewModel.RequestEnd += HideSideMenu;
    }

    public LoginViewModel LoginViewModel { get; }
    public SignUpViewModel SignUpViewModel { get; }
    public MultiplayerViewModel MultiplayerViewModel { get; }
    public LeaderboardViewModel LeaderboardViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }

    private SelectedMenuTab _selectedMenuTab = SelectedMenuTab.None;
    public SelectedMenuTab SelectedMenuTab
    {
        get => _selectedMenuTab;
        set
        {
            var previousTab = _selectedMenuTab;
            if (SetProperty(ref _selectedMenuTab, value,
                    onChanging: _ => OnDeactivation()))
            {
                _soundService.PlaySoundEffect(SoundEffectType.SmallMenuAnimation);
                SelectedTabChanged?.Invoke(this, (previousTab, value));
                OnActivation();
            }
        }
    }

    public RelayCommand LocalGameCommand { get; }
    public RelayCommand SelectEditorCommand { get; }
    
    public RelayCommand ShowSignUpCommand { get; }
    public RelayCommand ShowLoginCommand { get; }
    public RelayCommand ShowLobbiesCommand { get; }
    public RelayCommand ShowLeaderboardCommand { get; }
    public RelayCommand ShowSettingsCommand { get; }
    public RelayCommand ExitCommand { get; }

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
    }

    private void ShowLogin()
    {
        SelectedMenuTab = SelectedMenuTab.Login;
    }

    private void ShowSignUp()
    {
        SelectedMenuTab = SelectedMenuTab.SignUp;
    }

    private void ShowLeaderboard()
    {
        SelectedMenuTab = SelectedMenuTab.Leaderboard;
    }

    private void ShowSettings()
    {
        SelectedMenuTab = SelectedMenuTab.Settings;
    }

    private void Exit()
    {
        Application.Current.Shutdown();
    }

    public void OnActivation()
    {
        if (SelectedMenuTab == SelectedMenuTab.Lobby)
            MultiplayerViewModel.OnActivation();
        else if (SelectedMenuTab == SelectedMenuTab.SignUp)
            SignUpViewModel.OnActivation();
        else if  (SelectedMenuTab == SelectedMenuTab.Leaderboard)
            LeaderboardViewModel.OnActivation();
    }

    public void OnDeactivation()
    {
        if (SelectedMenuTab == SelectedMenuTab.Lobby)
            MultiplayerViewModel.OnDeactivation();
        else if (SelectedMenuTab == SelectedMenuTab.SignUp)
            SignUpViewModel.OnDeactivation();
        else if  (SelectedMenuTab == SelectedMenuTab.Leaderboard)
            LeaderboardViewModel.OnDeactivation();
    }

    private void HideSideMenu(object? sender, EventArgs e)
    {
        SelectedMenuTab = SelectedMenuTab.None;
    }
}