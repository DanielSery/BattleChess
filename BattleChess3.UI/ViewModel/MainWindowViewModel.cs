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
        PlayersViewModel playersViewModel,
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
        PlayersViewModel = playersViewModel;

        _playerService.PlayerWon += PlayerServiceOnPlayerWon;
        EditorViewModel.RequestSwitchToMainView += OnRequestSwitchToMainView;

        MenuViewModel.RequestSwitchToGame += OnRequestSwitchToGame;
        MenuViewModel.RequestSwitchToEditor += OnRequestSwitchToEditor;
        BoardViewModel.RequestSwitchToGame += OnRequestSwitchToGame;
        BoardViewModel.RequestSwitchToMenu += OnRequestSwitchToMainView;
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
    public PlayersViewModel PlayersViewModel { get; set; }
    public ILoadingService LoadingService { get; }
    public INotificationService NotificationService { get; }

    private async void PlayerServiceOnPlayerWon(object? sender, (Player? won, Player? lost) e)
    {
        if (e.won is null || e.lost is null)
            return;
        
        NotificationService.ShowMessage(ShownMessage.MessageType.Info, $"{e.won.Name} player won!");
        
        var result = await _multiplayerGameService.HandleWinAsync(e.won, e.lost);
        if (result.IsFailed)
        {
            NotificationService.ShowMessage(ShownMessage.MessageType.Error, result.Reasons.First().Message);
            return;
        }
        
        NotificationService.ShowMessage(ShownMessage.MessageType.Info, result.Value);
    }

    private void OnRequestSwitchToMainView(object? sender, EventArgs e)
    {
        MenuTabSelected = true;
    }

    private void OnRequestSwitchToGame(object? sender, EventArgs e)
    {
        GameTabSelected = true;
    }

    private void OnRequestSwitchToEditor(object? sender, EventArgs e)
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