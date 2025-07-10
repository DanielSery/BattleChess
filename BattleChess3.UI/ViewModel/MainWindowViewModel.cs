using System.Windows;
using BattleChess3.Game.Players;
using BattleChess3.Multiplayer;
using BattleChess3.UI.Services;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public sealed class MainWindowViewModel : ViewModelBase
{
    private bool _editorTabSelected;
    private bool _gameTabSelected;
    private bool _menuTabSelected = true;
    
    private readonly IPlayerService _playerService;
    private readonly IMultiplayerGameService _multiplayerGameService;

    public MainWindowViewModel(
        BoardViewModel boardViewModel,
        EditorViewModel editorViewModel,
        IPlayerService playerService,
        MenuViewModel menuViewModel,
        IMultiplayerGameService multiplayerGameService,
        INotificationService notificationService,
        ILoadingService loadingService)
    {
        _playerService = playerService;
        _multiplayerGameService = multiplayerGameService;
        
        NotificationService = notificationService;
        BoardViewModel = boardViewModel;
        EditorViewModel = editorViewModel;
        MenuViewModel = menuViewModel;
        LoadingService = loadingService;

        _playerService.PlayerWon += PlayerServiceOnPlayerWon;
        EditorViewModel.RequestSwitchToMainView += EditorViewModelOnRequestSwitchToMainView;

        MenuViewModel.RequestSwitchToGame += MenuViewModelOnRequestSwitchToGame;
        MenuViewModel.RequestSwitchToEditor += MenuViewModelOnRequestSwitchToEditor;
        BoardViewModel.RequestSwitchToGame += BoardViewModelOnRequestSwitchToGame;
    }

    public bool MenuTabSelected
    {
        get => _menuTabSelected;
        set => SetTabSelected(out _menuTabSelected, value);
    }

    public bool GameTabSelected
    {
        get => _gameTabSelected;
        set => SetTabSelected(out _gameTabSelected, value);
    }

    public bool EditorTabSelected
    {
        get => _editorTabSelected;
        set => SetTabSelected(out _editorTabSelected, value);
    }

    public MenuViewModel MenuViewModel { get; }
    public BoardViewModel BoardViewModel { get; }
    public EditorViewModel EditorViewModel { get; }
    public ILoadingService LoadingService { get; }
    public INotificationService NotificationService { get; }

    private void PlayerServiceOnPlayerWon(object? sender, (Player won, Player lost) e)
    {
        NotificationService.ShowMessage(ShownMessage.MessageType.Info, $"{e.won.Name} player won!");
        
        var result = _multiplayerGameService.HandleWinAsync(e.won, e.lost).Result;
        if (result.IsFailed)
        {
            NotificationService.ShowMessage(ShownMessage.MessageType.Error, result.Reasons.First().Message);
            return;
        }
        
        NotificationService.ShowMessage(ShownMessage.MessageType.Info, result.Value);
    }

    private void EditorViewModelOnRequestSwitchToMainView(object? sender, EventArgs e)
    {
        MenuTabSelected = true;
    }

    private void BoardViewModelOnRequestSwitchToGame(object? sender, EventArgs e)
    {
        GameTabSelected = true;
    }

    private void MenuViewModelOnRequestSwitchToGame(object? sender, EventArgs e)
    {
        GameTabSelected = true;
    }

    private void MenuViewModelOnRequestSwitchToEditor(object? sender, EventArgs e)
    {
        EditorTabSelected = true;
    }

    private void SetTabSelected(out bool selectedTab, bool value)
    {
        _menuTabSelected = false;
        _gameTabSelected = false;
        _editorTabSelected = false;
        selectedTab = value;

        BoardViewModel.ClearSelectedTile();
        RaisePropertyChanged(nameof(MenuTabSelected));
        RaisePropertyChanged(nameof(GameTabSelected));
        RaisePropertyChanged(nameof(EditorTabSelected));
    }
}