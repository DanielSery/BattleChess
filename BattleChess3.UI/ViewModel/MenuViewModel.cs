
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
        MultiplayerLobbyViewModel multiplayerLobbyViewModel)
    {
        _boardViewModel = boardViewModel;
        _mapsViewModel = mapsViewModel;
        TeamBoardViewModel = teamBoardViewModel;
        SignUpViewModel = signUpViewModel;
        LoginViewModel = loginViewModel;
        MultiplayerLobbyViewModel = multiplayerLobbyViewModel;
        
        LocalGameCommand = new RelayCommand(LocalGame);
        SelectEditorCommand = new RelayCommand(SelectEditor);
        ShowLobbiesCommand = new RelayCommand(ShowLobbies);
        ShowLoginCommand = new RelayCommand(ShowLogin);
        ShowSignUpCommand = new RelayCommand(ShowSignUp);
        RankedGameCommand = new RelayCommand(RankedGame);
        
        SignUpViewModel.RequestEndSignUp += HideSideMenu;
        LoginViewModel.RequestEndLogin += HideSideMenu;
    }

    public LoginViewModel LoginViewModel { get; }
    public SignUpViewModel SignUpViewModel { get; }
    public TeamBoardViewModel TeamBoardViewModel { get; }
    public MultiplayerLobbyViewModel MultiplayerLobbyViewModel { get; }

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

    public RelayCommand LocalGameCommand { get; }
    public RelayCommand SelectEditorCommand { get; }
    
    public RelayCommand ShowSignUpCommand { get; }
    public RelayCommand ShowLoginCommand { get; }
    public RelayCommand ShowLobbiesCommand { get; }
    public RelayCommand RankedGameCommand { get; }

    public event EventHandler? RequestSwitchToGame;
    public event EventHandler? RequestSwitchToEditor;
    
    private void RankedGame()
    {
    }

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
        MultiplayerLobbyViewModel.OnActivation();
        LobbyShown = true;
    }

    private void ShowLogin()
    {
        LoginShown = true;
    }

    private void ShowSignUp()
    {
        SignUpShown = true;
    }

    private void HideSideMenu(object? sender, EventArgs e)
    {
        SetTabSelected(out _, false); 
    }

    private void SetTabSelected(out bool selectedTab, bool value)
    {
        _loginShown = false;
        _signUpShown = false;
        _lobbyShown = false;
        selectedTab = value;

        RaisePropertyChanged(nameof(LoginShown));
        RaisePropertyChanged(nameof(SignUpShown));
        RaisePropertyChanged(nameof(LobbyShown));
    }
}