using BattleChess3.Game.Players;
using BattleChess3.Multiplayer;
using BattleChess3.UI.Editor;
using BattleChess3.UI.Game;
using BattleChess3.UI.Menu;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.MainWindow;

public sealed class MainWindowViewModel : ViewModelBase
{
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
    
    private SelectedMainWindowTab _selectedTab = SelectedMainWindowTab.Menu;
    public SelectedMainWindowTab SelectedTab
    {
        get => _selectedTab;
        set
        {
            var previousTab = _selectedTab;
            if (SetProperty(ref _selectedTab, value))
            {
                BoardViewModel.ClearSelectedTile();
                SelectedTabChanged?.Invoke(this, (previousTab, value));
            }
        }
    }

    public MenuViewModel MenuViewModel { get; }
    public BoardViewModel BoardViewModel { get; }
    public EditorViewModel EditorViewModel { get; }
    public PlayersViewModel PlayersViewModel { get; }
    public ILoadingService LoadingService { get; }
    public INotificationService NotificationService { get; }

    public event EventHandler<(SelectedMainWindowTab, SelectedMainWindowTab)>? SelectedTabChanged;

    private async void PlayerServiceOnPlayerWon(object? sender, (bool notifyOther, WinType winType, Player? won, Player? lost) e)
    {
        if (e.won is null || e.lost is null)
            return;

        ShownMessage.MessageType messageType = ShownMessage.MessageType.Info;
        if (!_playerService.IsMultiplayer)
        {
            messageType = ShownMessage.MessageType.Info;
        }
        else if (e.won.Index == 1)
        {
            messageType = ShownMessage.MessageType.Success;
        }
        else if (e.won.Index == 2)
        {
            messageType = ShownMessage.MessageType.Warning;
        }

        if (e.winType == WinType.NotResponding)
        {
            NotificationService.ShowMessage(messageType, $"{e.won.Name} won! {e.lost.Name} did not play in time.");
        }
        else if (e.winType == WinType.Surrender)
        {
            NotificationService.ShowMessage(messageType, $"{e.won.Name} won! {e.lost.Name} surrendered.");
        }
        else if (e.winType == WinType.OutOfTime)
        {
            NotificationService.ShowMessage(messageType, $"{e.won.Name} won! {e.lost.Name} ran out of time.");
        }
        else if (e.winType == WinType.CapturedKing)
        {
            NotificationService.ShowMessage(messageType, $"{e.won.Name} won! {e.lost.Name}'s king was captured.");
        }
        
        var result = await _multiplayerGameService.HandleWinAsync(e.notifyOther, e.winType, e.won, e.lost);
        if (result.IsFailed)
        {
            NotificationService.ShowMessage(ShownMessage.MessageType.Error, result.Reasons.First().Message);
            return;
        }

        if (!string.IsNullOrEmpty(result.Value))
        {
            NotificationService.ShowMessage(messageType, result.Value);
        }
    }

    private void OnRequestSwitchToMainView(object? sender, EventArgs e)
    {
        SelectedTab = SelectedMainWindowTab.Menu;
    }

    private void OnRequestSwitchToGame(object? sender, EventArgs e)
    {
        SelectedTab = SelectedMainWindowTab.Game;
    }

    private void OnRequestSwitchToEditor(object? sender, EventArgs e)
    {
        SelectedTab = SelectedMainWindowTab.Editor;
    }
}