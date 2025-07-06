using System.Windows;
using BattleChess3.Game.Players;
using BattleChess3.Maps;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BattleChess3.UI.ViewModel;

public sealed class MainWindowViewModel : ViewModelBase
{
    private bool _editorTabSelected;
    private bool _gameTabSelected;
    private bool _menuTabSelected;
    private bool _optionsTabSelected;

    public MainWindowViewModel(
        MapsViewModel mapsViewModel,
        BoardViewModel boardViewModel,
        EditorViewModel editorViewModel,
        MultiplayerViewModel multiplayerViewModel,
        TeamBoardViewModel teamBoardViewModel,
        IPlayerService playerService)
    {
        MapsViewModel = mapsViewModel;
        BoardViewModel = boardViewModel;
        EditorViewModel = editorViewModel;
        MultiplayerViewModel = multiplayerViewModel;
        PlayerService = playerService;
        TeamBoardViewModel = teamBoardViewModel;

        NewGameCommand = new RelayCommand(NewGame);
        HostGameCommand = new RelayCommand(HostGame);
        JoinGameCommand = new RelayCommand(JoinGame);
        EditorCommand = new RelayCommand(SelectEditor);
        SelectOptionsCommand = new RelayCommand(() => OptionsTabSelected = true);
        CloseApplicationCommand = new RelayCommand(CloseApplication);
        
        PlayerService.PlayerWon += PlayerServiceOnPlayerWon;
        EditorViewModel.RequestSwitchToMainView += EditorViewModelOnRequestSwitchToMainView;
    }


    public bool MenuTabSelected
    {
        get => _menuTabSelected;
        set => SetTabSelected(out _menuTabSelected);
    }

    public bool GameTabSelected
    {
        get => _gameTabSelected;
        set => SetTabSelected(out _gameTabSelected);
    }

    public bool OptionsTabSelected
    {
        get => _optionsTabSelected;
        set => SetTabSelected(out _optionsTabSelected);
    }

    public bool EditorTabSelected
    {
        get => _editorTabSelected;
        set
        {
            SetTabSelected(out _editorTabSelected);
            BoardViewModel.ClearSelectedTile();
        }
    }

    public MapsViewModel MapsViewModel { get; }
    public BoardViewModel BoardViewModel { get; }
    public EditorViewModel EditorViewModel { get; }
    public MultiplayerViewModel MultiplayerViewModel { get; }
    public TeamBoardViewModel TeamBoardViewModel { get; }
    public IPlayerService PlayerService { get; }

    public RelayCommand NewGameCommand { get; }
    public RelayCommand HostGameCommand { get; }
    public RelayCommand JoinGameCommand { get; }
    public RelayCommand EditorCommand { get; }
    public RelayCommand SelectOptionsCommand { get; }
    public RelayCommand CloseApplicationCommand { get; }

    private void HostGame()
    {
        GameTabSelected = true;
        MultiplayerViewModel.HostAndCopyCommand.Execute(null);
    }

    private void NewGame()
    {
        BoardViewModel.SinglePlayerLoadMap(MapsViewModel.TeamMap);
        GameTabSelected = true;
    }

    private void SelectEditor()
    {
        EditorTabSelected = true;
    }

    private void JoinGame()
    {
        MultiplayerViewModel.PasteAndJoinCommand.Execute(null);
        GameTabSelected = true;
    }

    private void PlayerServiceOnPlayerWon(object? sender, int e)
    {
        var playerColor = e == 1 ? "Red" : "Blue";
        MessageBox.Show($"{playerColor} player won!", "Player won");
    }

    private void EditorViewModelOnRequestSwitchToMainView(object? sender, EventArgs e)
    {
        MenuTabSelected = true;
    }

    private static void CloseApplication()
    {
        Application.Current.Shutdown();
    }

    private void SetTabSelected(out bool selectedTab)
    {
        _menuTabSelected = false;
        _gameTabSelected = false;
        _optionsTabSelected = false;
        _editorTabSelected = false;
        selectedTab = true;

        RaisePropertyChanged(nameof(MenuTabSelected));
        RaisePropertyChanged(nameof(GameTabSelected));
        RaisePropertyChanged(nameof(OptionsTabSelected));
        RaisePropertyChanged(nameof(EditorTabSelected));
    }
}