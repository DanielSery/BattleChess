
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class MenuViewModel : ViewModelBase
{
    private readonly MapsViewModel _mapsViewModel;
    private readonly BoardViewModel _boardViewModel;
    private readonly MultiplayerViewModel _multiplayerViewModel;

    public MenuViewModel(
        MultiplayerViewModel multiplayerViewModel,
        BoardViewModel boardViewModel,
        MapsViewModel mapsViewModel,
        TeamBoardViewModel teamBoardViewModel,
        SignUpViewModel signUpViewModel,
        LoginViewModel loginViewModel)
    {
        _multiplayerViewModel = multiplayerViewModel;
        _boardViewModel = boardViewModel;
        _mapsViewModel = mapsViewModel;
        TeamBoardViewModel = teamBoardViewModel;
        SignUpViewModel = signUpViewModel;
        LoginViewModel = loginViewModel;
        
        NewGameCommand = new RelayCommand(NewGame);
        HostGameCommand = new RelayCommand(HostGame);
        JoinGameCommand = new RelayCommand(JoinGame);
        SelectEditorCommand = new RelayCommand(SelectEditor);
        ShowLoginCommand = new RelayCommand(ShowLogin);
        ShowSignUpCommand = new RelayCommand(ShowSignUp);
    }

    public LoginViewModel LoginViewModel { get; }
    public SignUpViewModel SignUpViewModel { get; }
    public TeamBoardViewModel TeamBoardViewModel { get; }

    private bool _loginShown;
    public bool LoginShown
    {
        get => _loginShown;
        set => SetProperty(ref _loginShown, value);
    }

    private bool _signUpShown;
    public bool SignUpShown
    {
        get => _signUpShown;
        set => SetProperty(ref _signUpShown, value);
    }

    public RelayCommand NewGameCommand { get; }
    public RelayCommand HostGameCommand { get; }
    public RelayCommand JoinGameCommand { get; }
    public RelayCommand SelectEditorCommand { get; }
    public RelayCommand ShowSignUpCommand { get; set; }
    public RelayCommand ShowLoginCommand { get; }

    public event EventHandler? RequestSwitchToGame;
    public event EventHandler? RequestSwitchToEditor;
    
    private void JoinGame()
    {
        _multiplayerViewModel.PasteAndJoinCommand.Execute(null);
        RequestSwitchToGame?.Invoke(this, EventArgs.Empty);
    }

    private void HostGame()
    {
        RequestSwitchToGame?.Invoke(this, EventArgs.Empty);
        _multiplayerViewModel.HostAndCopyCommand.Execute(null);
    }

    private void NewGame()
    {
        _boardViewModel.SinglePlayerLoadMap(_mapsViewModel.TeamMap);
        RequestSwitchToGame?.Invoke(this, EventArgs.Empty);
    }

    private void SelectEditor()
    {
        RequestSwitchToEditor?.Invoke(this, EventArgs.Empty);
    }

    private void ShowLogin()
    {
        SetTabSelected(out _loginShown);
    }

    private void ShowSignUp()
    {
        SetTabSelected(out _signUpShown);
    }

    private void HideSideMenu()
    {
        SetTabSelected(out _);
    }

    private void SetTabSelected(out bool selectedTab)
    {
        _loginShown = false;
        _signUpShown = false;
        selectedTab = true;

        RaisePropertyChanged(nameof(LoginShown));
        RaisePropertyChanged(nameof(SignUpShown));
    }
}