using System.Windows;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;
using BattleChess3.Maps;
using BattleChess3.Multiplayer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BattleChess3.UI.ViewModel;

public sealed class MultiplayerViewModel : ViewModelBase
{
    private readonly BoardViewModel _boardViewModel;
    private readonly IMultiplayerService _multiplayerService;
    private readonly IPlayerService _playerService;
    private readonly TeamBoardViewModel _teamBoardViewModel;
    private uint? _gameId;

    public MultiplayerViewModel(
        TeamBoardViewModel teamBoardViewModel,
        BoardViewModel boardViewModel,
        IMultiplayerService multiplayerService,
        IPlayerService playerService)
    {
        _teamBoardViewModel = teamBoardViewModel;
        _boardViewModel = boardViewModel;
        _multiplayerService = multiplayerService;
        _playerService = playerService;

        HostAndCopyCommand = new RelayCommand(HostGame, CanConnect);
        PasteAndJoinCommand = new RelayCommand(JoinGame, CanConnect);
        StopCommand = new RelayCommand(StopMultiplayer, IsConnected);

        SubscribeToEvents();
    }

    public bool IsConnected => _multiplayerService.IsHost || _multiplayerService.IsGuest;
    public bool CanConnect => _multiplayerService is { IsHost: false, IsGuest: false };

    public RelayCommand HostAndCopyCommand { get; }
    public RelayCommand PasteAndJoinCommand { get; }
    public RelayCommand StopCommand { get; }

    private void SubscribeToEvents()
    {
        _multiplayerService.RequestPlayMove += MultiplayerServiceOnRequestPlayMove;
        _multiplayerService.RequestLoadMap += RemoteRequestedLoadMap;
        _multiplayerService.RequestDisplayMessage += RemoteRequestedDisplayMessage;
        _boardViewModel.RequestMove += LocalRequestMove;
        _boardViewModel.RequestLoadMap += LocalRequestLoadMap;
    }

    private void RemoteRequestedDisplayMessage(object? sender, string e)
    {
        Application.Current.Dispatcher.Invoke(() => MessageBox.Show(e));
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
        _boardViewModel.AutomaticLoadMap(e);
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

    private void HostGame()
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
        var gameIdTask = _multiplayerService.Host(false, isHostStarting, map);
        gameIdTask.ContinueWith(task =>
        {
            Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(task.Result));
            _multiplayerService.WaitForHostConfirmation(isHostStarting, map);
            RaiseCanExecuteChanged();
        });;
    }

    private void RaiseCanExecuteChanged()
    {
        HostAndCopyCommand.RaiseCanExecuteChanged();
        PasteAndJoinCommand.RaiseCanExecuteChanged();
        StopCommand.RaiseCanExecuteChanged();

        RaisePropertyChanged(nameof(IsConnected));
        RaisePropertyChanged(nameof(CanConnect));
    }
}