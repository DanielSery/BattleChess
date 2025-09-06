using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Game;
using CrownsGuard.Game.GameBoard;
using CrownsGuard.Game.Helpers;
using CrownsGuard.Game.Players;
using CrownsGuard.Game.Timers;
using CrownsGuard.Multiplayer.Game;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.UI.Services;
using CrownsGuard.UI.Shared;
using CommunityToolkit.Mvvm.Input;
using CrownsGuard.Maps.BoardBlueprints;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Game;

public sealed class BoardViewModel : ViewModelBase
{
    private readonly IGameService _gameService;
    private readonly IBoardLoader _boardLoader;
    private readonly IMultiplayerGameService _multiplayerGameService;
    private readonly ISoundService _soundService;

    private TileViewModel _mouseOnTile = TileViewModel.None;
    private TileViewModel _selectedTile = TileViewModel.None;
    private readonly IBoard _board;

    public BoardViewModel(
        IGameService gameService,
        IBoardLoader boardLoader,
        IMultiplayerGameService multiplayerGameService,
        ISoundService soundService)
    {
        _gameService = gameService;
        _boardLoader = boardLoader;
        _multiplayerGameService = multiplayerGameService;
        _soundService = soundService;

        SurrenderCommand = new RelayCommand(Surrender);
        PlayTileCommand = new AsyncRelayCommand<TileViewModel>(PlayTile);
        MouseEnterCommand = new RelayCommand<TileViewModel>(MouseEnterTile);
        MouseExitCommand = new RelayCommand<TileViewModel>(MouseExitTile);

        Tiles = Enumerable.Range(0, Constants.FullBoardTilesCount)
            .Select<int, TileViewModel>(index => new TileViewModel(Position.FromIndex(index)))
            .ToArray();
        _board = new Board(Tiles.Cast<ITile>().ToArray());

        SinglePlayerLoadMap(BoardBlueprint.ChessTeam);
        _multiplayerGameService.RequestPlayMove += MultiplayerGameServiceOnRequestPlayMove;
    }

    public TileViewModel SelectedTile
    {
        get => _selectedTile;
        private set
        {
            _selectedTile.IsSelected = false;
            SetProperty(ref _selectedTile, value);
            RaisePropertyChanged(nameof(TileInfo));
            value.IsSelected = true;
        }
    }

    public TileViewModel MouseOnTile
    {
        get => _mouseOnTile;
        private set
        {
            _mouseOnTile.IsMouseOver = false;
            SetProperty(ref _mouseOnTile, value);

            if (_selectedTile.Equals(TileViewModel.None))
            {
                ClearPossibleActions();
                SetPossibleActions(value, false);
            }

            RaisePropertyChanged(nameof(TileInfo));
            value.IsMouseOver = true;
        }
    }

    public TileViewModel TileInfo
    {
        get => SelectedTile.Equals(TileViewModel.None)
            ? SelectedTile
            : MouseOnTile;
    }

    public int BoardWidth => Constants.BoardLength;
    public TileViewModel[] Tiles { get; }

    public RelayCommand SurrenderCommand { get; }
    public AsyncRelayCommand<TileViewModel> PlayTileCommand { get; }
    public RelayCommand<TileViewModel> MouseEnterCommand { get; }
    public RelayCommand<TileViewModel> MouseExitCommand { get; }

    public event EventHandler? RequestSwitchToMenu;
    public event EventHandler? RequestSwitchToGame;

    public void SinglePlayerLoadMap(BoardBlueprint map)
    {
        _gameService.StartGame(
            new ControlledPlayerInfo(Player.White, "Red player"),
            new ControlledPlayerInfo(Player.Black, "Blue player"),
            map.StartingPlayer);

        _boardLoader.LoadBoardExtendedFor2Players(_board, map);
        RequestSwitchToGame?.Invoke(this, EventArgs.Empty);
    }

    public void MultiplayerLoadMap(
        MultiplayerGameType gameType,
        string? gameId,
        IOnlinePlayerInfo player1,
        IOnlinePlayerInfo player2,
        BoardBlueprint map,
        bool hasTimer)
    {
        if (hasTimer)
        {
            player1.SetTimer(new ChessTimer(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10)));
            player2.SetTimer(new ChessTimer(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10)));
        }

        player1.SetGameService(_multiplayerGameService);
        player2.SetGameService(_multiplayerGameService);

        _multiplayerGameService.StartGame(gameType, gameId);
        _gameService.StartGame(
            player1, player2,
            map.StartingPlayer);
        if (_gameService.CurrentPlayerInfo is IAutomaticallyControlledPlayerInfo automaticallyControlledPlayer)
            _ = automaticallyControlledPlayer.HandleAutomaticTurnAsync();

        _boardLoader.LoadBoard(_board, map);
        RequestSwitchToGame?.Invoke(this, EventArgs.Empty);
    }

    private void Surrender()
    {
        if (_gameService is { GameRunning: true, BlackPlayer: IOnlinePlayerInfo })
        {
            _soundService.PlaySoundEffect(SoundEffectType.Button);
            _gameService.Surrender();
        }
        else
        {
            RequestSwitchToMenu?.Invoke(this, EventArgs.Empty);
        }
    }

    private async Task PlayTile(TileViewModel? clickedTile)
    {
        ArgumentNullException.ThrowIfNull(clickedTile);
        if (clickedTile.PossibleAction.ActionType != FigureActionTypes.None)
        {
            _gameService.EndTurn();
            await _multiplayerGameService.PlayedMoveAsync(
                SelectedTile.RelativePosition,
                clickedTile.RelativePosition,
                _gameService.CurrentPlayerInfo.Timer.LastTurnElapsedTime,
                CancellationToken.None);
            
            clickedTile.PossibleAction.Action.Invoke();
            ClearPossibleActions();

            _soundService.PlaySoundEffect(SoundEffectType.ChessFigure);
            SelectedTile = TileViewModel.None;
            _gameService.StartTurn();
            if (_gameService.CurrentPlayerInfo is IAutomaticallyControlledPlayerInfo automaticallyControlledPlayer)
                _ = automaticallyControlledPlayer.HandleAutomaticTurnAsync();
        }
        else if (clickedTile.Figure.Owner.Equals(_gameService.CurrentPlayerInfo))
        {
            SelectedTile = clickedTile;
            ClearPossibleActions();
            SetPossibleActions(clickedTile, false);
        }
        else
        {
            SelectedTile = TileViewModel.None;
            ClearPossibleActions();
        }
    }


    private void ClearPossibleActions()
    {
        foreach (var tile in Tiles)
        {
            tile.PossibleAction = FigureAction.None;
        }
    }

    private void SetPossibleActions(TileViewModel clickedTile, bool automatic)
    {
        if (!_gameService.GameRunning)
            return;

        if (_gameService.CurrentPlayerInfo is IAutomaticallyControlledPlayerInfo && !automatic)
            return;

        if (!_gameService.CurrentPlayerInfo.Equals(clickedTile.Figure.Owner))
            return;

        var relativeBoard = GetPlayerRelativeBoard(clickedTile.Figure.Owner.Player, Tiles);
        var relativeClickedTile = clickedTile.GetRelativeTile(clickedTile.Figure.Owner.Player);
        var possibleActions = clickedTile.Figure.Type.GetPossibleActions(relativeClickedTile, relativeBoard);

        foreach (var possibleAction in possibleActions)
        {
            Tiles[possibleAction.TargetPosition.GetIndex()].PossibleAction = possibleAction;
        }
    }

    private static IBoard GetPlayerRelativeBoard(Player player, IReadOnlyList<ITile> board)
    {
        var povBoard = new ITile[Constants.FullBoardTilesCount];
        var absoluteBoard = board.Select(x => x.GetRelativeTile(player)).ToArray();

        for (var i = 0; i < Constants.BoardLength; i++)
        for (var j = 0; j < Constants.BoardLength; j++)
        {
            var position = new Position(j, i);
            povBoard[RelativePositionHelper.GetRelative(player, position).GetIndex()] =
                absoluteBoard[position.GetIndex()];
        }

        return new Board(povBoard);
    }

    private void MouseEnterTile(TileViewModel? tile)
    {
        ArgumentNullException.ThrowIfNull(tile);
        MouseOnTile = tile;
    }

    private void MouseExitTile(TileViewModel? tile)
    {
        ArgumentNullException.ThrowIfNull(tile);
        if (MouseOnTile == tile)
        {
            MouseOnTile = TileViewModel.None;
        }
    }

    public void OnActivation()
    {
    }

    public void OnDeactivation()
    {
        SelectedTile = TileViewModel.None;
        ClearPossibleActions();
    }

    private void MultiplayerGameServiceOnRequestPlayMove(object? sender,
        (Position from, Position to, TimeSpan turnTimeSpent) e)
    {
        SelectedTile = TileViewModel.None;
        ClearPossibleActions();

        switch (e.from.GetIndex())
        {
            case IMultiplayerGameService.NotRespondingMessage:
                _gameService.PlayerWin(_gameService.WhitePlayer, WinType.NotResponding, true);
                return;
            case IMultiplayerGameService.NotRespondingLostMessage:
                _gameService.PlayerWin(_gameService.BlackPlayer, WinType.NotResponding, false);
                return;
            case IMultiplayerGameService.OutOfTimeMessage:
                _gameService.PlayerWin(_gameService.WhitePlayer, WinType.OutOfTime, false);
                return;
            case IMultiplayerGameService.SurrenderMessage:
                _gameService.PlayerWin(_gameService.WhitePlayer, WinType.Surrender, false);
                return;
        }

        var fromTile = Tiles[e.from.GetIndex()];
        SelectedTile = fromTile;
        SetPossibleActions(fromTile, true);

        _gameService.EndTurn(e.turnTimeSpent);
        var toTile = Tiles[e.to.GetIndex()];
        toTile.PossibleAction.Action.Invoke();
        _soundService.PlaySoundEffect(SoundEffectType.ChessFigure);
        SelectedTile = TileViewModel.None;
        _gameService.StartTurn();
        ClearPossibleActions();
    }
}
  