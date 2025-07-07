using System.Windows;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Maps;
using BattleChess3.Multiplayer;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public sealed class MultiplayerViewModel : ViewModelBase
{
    private readonly BoardViewModel _boardViewModel;
    private readonly IMultiplayerService _multiplayerService;
    private readonly TeamBoardViewModel _teamBoardViewModel;

    public MultiplayerViewModel(
        TeamBoardViewModel teamBoardViewModel,
        BoardViewModel boardViewModel,
        IMultiplayerService multiplayerService)
    {
        _teamBoardViewModel = teamBoardViewModel;
        _boardViewModel = boardViewModel;
        _multiplayerService = multiplayerService;

        HostAndCopyCommand = new AsyncRelayCommand(HostGame, CanConnect);
        PasteAndJoinCommand = new RelayCommand(JoinGame, CanConnect);
        StopCommand = new RelayCommand(StopMultiplayer, IsConnected);

        SubscribeToEvents();
    }

    public bool IsConnected() => _multiplayerService.IsHost || _multiplayerService.IsGuest;
    public bool CanConnect() => _multiplayerService is { IsHost: false, IsGuest: false };

    public AsyncRelayCommand HostAndCopyCommand { get; }
    public RelayCommand PasteAndJoinCommand { get; }
    public RelayCommand StopCommand { get; }

    private void SubscribeToEvents()
    {
        _multiplayerService.RequestPlayMove += MultiplayerServiceOnRequestPlayMove;
        _multiplayerService.RequestLoadMap += RemoteRequestedLoadMap;
        _boardViewModel.RequestMove += LocalRequestMove;
        _boardViewModel.RequestLoadMap += LocalRequestLoadMap;
    }

    private void LocalRequestLoadMap(object? sender, MapBlueprint e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _multiplayerService.Stop();
            RaiseCanExecuteChanged();
        });
    }

    private void LocalRequestMove(object? sender, (Position from, Position to) e)
    {
        Application.Current.Dispatcher.Invoke(() => _multiplayerService.PlayedMove(e.from, e.to));
    }

    private void RemoteRequestedLoadMap(object? sender, MapBlueprint e)
    {
        _boardViewModel.MultiplayerLoadMap(e);
    }

    private void MultiplayerServiceOnRequestPlayMove(object? sender, (Position from, Position to) e)
    {
        _boardViewModel.RemotePlayTurn(
            _boardViewModel.Tiles[e.from.Index],
            _boardViewModel.Tiles[e.to.Index]);
    }

    private void StopMultiplayer()
    {
        _multiplayerService.Stop();
        RaiseCanExecuteChanged();
    }

    private void JoinGame()
    {
        var map = new MapBlueprint
        {
            Figures = _teamBoardViewModel.Tiles.Select(x => new FigureIdentifier
            {
                PlayerId = x.Figure.Owner.Id,
                FigureId = ((IFigureType)x.Figure).FigureId,
                IsKing = x.Figure.IsKing
            }).ToArray(),
        };
        
        _multiplayerService.Join(Clipboard.GetText(), map);
        RaiseCanExecuteChanged();
    }

    private async Task HostGame()
    {
        var map = new MapBlueprint
        {
            Figures = _teamBoardViewModel.Tiles.Select(x => new FigureIdentifier
            {
                PlayerId = x.Figure.Owner.Id,
                FigureId = ((IFigureType)x.Figure).FigureId,
                IsKing = x.Figure.IsKing
            }).ToArray(),
        };

        var random = new Random();
        var isHostStarting = random.Next(0, 1) == 1;
        var gameId = await _multiplayerService.Host(false, isHostStarting, map);
        Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(gameId));
        await _multiplayerService.WaitForHostConfirmation(isHostStarting, map);
        RaiseCanExecuteChanged();
    }

    private void RaiseCanExecuteChanged()
    {
        HostAndCopyCommand.NotifyCanExecuteChanged();
        PasteAndJoinCommand.NotifyCanExecuteChanged();
        StopCommand.NotifyCanExecuteChanged();

        RaisePropertyChanged(nameof(IsConnected));
        RaisePropertyChanged(nameof(CanConnect));
    }
}