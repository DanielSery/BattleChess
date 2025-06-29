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
    private uint? _gameId;

    public MultiplayerViewModel(
        BoardViewModel boardViewModel,
        IMultiplayerService multiplayerService,
        IPlayerService playerService)
    {
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

    public uint? GameId
    {
        get => _gameId;
        set => Set(ref _gameId, value);
    }

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
            _boardViewModel.Tiles[e.from],
            _boardViewModel.Tiles[e.to]);
    }

    public void SetGameId(uint gameId)
    {
        GameId = gameId;
        RaiseCanExecuteChanged();
    }

    private void StopMultiplayer()
    {
        _multiplayerService.Stop();
        RaiseCanExecuteChanged();
    }

    private void JoinGame()
    {
        if (!uint.TryParse(Clipboard.GetText(), out var gameId))
            return; 
        
        GameId = gameId;
        _multiplayerService.Join(GameId);
        RaiseCanExecuteChanged();
    }

    private void HostGame()
    {
        var map = new MapBlueprint
        {
            Figures = _boardViewModel.Tiles.Select(x => new FigureIdentifier
            {
                PlayerId = x.Figure.Owner.Id,
                UniqueUnitId = ((IFigureType)x.Figure).UniqueFigureId
            }).ToArray(),
            StartingPlayer = _playerService.CurrentPlayer.Id
        };

        var guid = Guid.NewGuid();
        var bytes = guid.ToByteArray();
        GameId = BitConverter.ToUInt32(bytes, 0);
        Clipboard.SetText(GameId.ToString() ?? string.Empty);
        _multiplayerService.Host(GameId.Value, map);
        RaiseCanExecuteChanged();
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