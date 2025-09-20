using CrownsGuard.Multiplayer.Players;
using CommunityToolkit.Mvvm.Input;
using CrownsGuard.UI.Services;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Editor;

public sealed class EditorViewModel : ViewModelBase
{
    private readonly IMultiplayerPlayerService _playerService;
    private readonly ILoadingService _loadingService;
    private readonly INotificationService _notificationService;

    public EditorViewModel(
        EditorUnitsViewModel editorUnits,
        TeamBoardViewModel teamBoard,
        IMultiplayerPlayerService playerService,
        ILoadingService loadingService,
        INotificationService notificationService)
    {
        _playerService = playerService;
        _loadingService = loadingService;
        _notificationService = notificationService;
        
        EditorUnits = editorUnits;
        TeamBoard = teamBoard;

        SaveGameCommand = new AsyncRelayCommand(SaveGameAsync);
        CancelCommand = new RelayCommand(Cancel);
    }

    public EditorUnitsViewModel EditorUnits { get; }
    public TeamBoardViewModel TeamBoard { get; }

    public AsyncRelayCommand SaveGameCommand { get; }
    public RelayCommand CancelCommand { get; }

    public event EventHandler? RequestSwitchToMenu;

    private async Task SaveGameAsync()
    {
        if (_playerService.LoggedInPlayer is not null)
        {
            using var updatingPlayerMap = _loadingService.StartLoadingOperation("Updating player map");
            var result = await _playerService.UpdateCurrentPlayerMapAsync(TeamBoard.GetMapBlueprint().Figures, updatingPlayerMap.CancellationToken);
            if (result.IsFailed)
            {
                _notificationService.ShowMessage(ShownMessage.MessageType.Warning, result.Errors[0].Message);
                TeamBoard.SaveMap();
            }
        }
        else
        {
            TeamBoard.SaveMap();
        }
        
        RequestSwitchToMenu?.Invoke(this, EventArgs.Empty);
    }

    private void Cancel()
    {
        TeamBoard.Discard();
        RequestSwitchToMenu?.Invoke(this, EventArgs.Empty);
    }
}