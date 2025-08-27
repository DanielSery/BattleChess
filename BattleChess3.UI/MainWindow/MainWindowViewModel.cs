using BattleChess3.Game;
using BattleChess3.Game.Players;
using BattleChess3.Multiplayer;
using BattleChess3.UI.Editor;
using BattleChess3.UI.Game;
using BattleChess3.UI.Menu;
using BattleChess3.UI.Multiplayer;
using BattleChess3.UI.Services;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.MainWindow;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly IGameService _gameService;
    private readonly IMultiplayerGameService _multiplayerGameService;
    private readonly ISoundService _soundService;

    public MainWindowViewModel(
        BoardViewModel boardViewModel,
        EditorViewModel editorViewModel,
        IGameService gameService,
        MenuViewModel menuViewModel,
        GameViewModel gameViewModel,
        IMultiplayerGameService multiplayerGameService,
        INotificationService notificationService,
        ILoadingService loadingService,
        ISoundService soundService)
    {
        _gameService = gameService;
        _multiplayerGameService = multiplayerGameService;
        _soundService = soundService;
        
        NotificationService = notificationService;
        BoardViewModel = boardViewModel;
        EditorViewModel = editorViewModel;
        MenuViewModel = menuViewModel;
        LoadingService = loadingService;
        GameViewModel = gameViewModel;

        _gameService.PlayerWon += GameServiceOnGameWon;
        EditorViewModel.RequestSwitchToMenu += OnRequestSwitchToMenu;
        MenuViewModel.RequestSwitchToGame += OnRequestSwitchToGame;
        MenuViewModel.RequestSwitchToEditor += OnRequestSwitchToEditor;
        BoardViewModel.RequestSwitchToGame += OnRequestSwitchToGame;
        BoardViewModel.RequestSwitchToMenu += OnRequestSwitchToMenu;
    }

    private SelectedMainWindowTab _selectedTab = SelectedMainWindowTab.Menu;
    public SelectedMainWindowTab SelectedTab
    {
        get => _selectedTab;
        set
        {
            var previousTab = _selectedTab;
            if (SetProperty(ref _selectedTab, value,
                    onChanging: _ => OnDeactivation()))
            {
                if (value == SelectedMainWindowTab.Game)
                {
                    _soundService.PauseBackgroundMusic();
                }
                else
                {
                    _soundService.ContinueBackgroundMusic();
                }
                
                _soundService.PlaySoundEffect(SoundEffectType.MenuAnimation);
                SelectedTabChanged?.Invoke(this, (previousTab, value));
                OnActivation();
            }
        }
    }

    public MenuViewModel MenuViewModel { get; }
    public BoardViewModel BoardViewModel { get; }
    public EditorViewModel EditorViewModel { get; }
    public GameViewModel GameViewModel { get; }
    public ILoadingService LoadingService { get; }
    public INotificationService NotificationService { get; }

    public event EventHandler<(SelectedMainWindowTab, SelectedMainWindowTab)>? SelectedTabChanged;

    private async void GameServiceOnGameWon(object? sender, (bool notifyOther, WinType winType, PlayerInfo? won, PlayerInfo? lost) e)
    {
        if (e.won is null || e.lost is null)
            return;

        var messageType = ShownMessage.MessageType.Info;
        if (!_gameService.IsMultiplayer)
        {
            messageType = ShownMessage.MessageType.Info;
        }
        else if (e.won.Player == Player.White)
        {
            messageType = ShownMessage.MessageType.Success;
        }
        else if (e.won.Player == Player.Black)
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
        }
        else if (!string.IsNullOrEmpty(result.Value))
        {
            NotificationService.ShowMessage(messageType, result.Value);
        }
        
        var unlockedUnit = await EditorViewModel.EditorUnits.PossiblyUnlockUnit(e.won.Player == Player.White);
        if (!string.IsNullOrEmpty(unlockedUnit))
        {
            NotificationService.ShowMessage(ShownMessage.MessageType.Success, $"Unlocked {unlockedUnit}.");
        }
    }

    private void OnActivation()
    {
        if (SelectedTab == SelectedMainWindowTab.Game)
            BoardViewModel.OnActivation();
        else if (SelectedTab == SelectedMainWindowTab.Menu)
            MenuViewModel.OnActivation();
    }

    private void OnDeactivation()
    {
        if (SelectedTab == SelectedMainWindowTab.Game)
            BoardViewModel.OnDeactivation();
        else if (SelectedTab == SelectedMainWindowTab.Menu)
            MenuViewModel.OnDeactivation();
    }

    private void OnRequestSwitchToMenu(object? sender, EventArgs e)
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