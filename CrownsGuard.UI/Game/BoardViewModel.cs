using CrownsGuard.Core;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Game;
using CrownsGuard.Game.Players;
using CrownsGuard.Game.Timers;
using CrownsGuard.Multiplayer.Game;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.UI.Services;
using CrownsGuard.UI.Shared;
using CommunityToolkit.Mvvm.Input;
using CrownsGuard.AI;
using CrownsGuard.Core.Figures;
using CrownsGuard.FigureDefinitions.Utilities;
using CrownsGuard.Maps.BoardBlueprints;
using CrownsGuard.Maps.Figures;
using CrownsGuard.Maps.GameBoard;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Game;

public sealed class BoardViewModel : ViewModelBase
{
    private readonly IGameService _gameService;
    private readonly IBoardLoader _boardLoader;
    private readonly IMultiplayerGameService _multiplayerGameService;
    private readonly ISoundService _soundService;
    private readonly IFigureCreator _figureCreator;

    private TileInfoViewModel _mouseOnTileInfo = TileInfoViewModel.None;
    private TileInfoViewModel _selectedTileInfo = TileInfoViewModel.None;
    private readonly IBoardInfo _boardInfo;

    public BoardViewModel(
        IGameService gameService,
        IBoardLoader boardLoader,
        IMultiplayerGameService multiplayerGameService,
        ISoundService soundService,
        IFigureCreator figureCreator)
    {
        _gameService = gameService;
        _boardLoader = boardLoader;
        _multiplayerGameService = multiplayerGameService;
        _soundService = soundService;
        _figureCreator = figureCreator;

        SurrenderCommand = new RelayCommand(Surrender);
        PlayTileCommand = new AsyncRelayCommand<TileInfoViewModel>(PlayTile);
        MouseEnterCommand = new RelayCommand<TileInfoViewModel>(MouseEnterTile);
        MouseExitCommand = new RelayCommand<TileInfoViewModel>(MouseExitTile);

        Tiles = Enumerable.Range(0, Constants.FullBoardTilesCount)
            .Select<int, TileInfoViewModel>(index => new TileInfoViewModel(Position.FromIndex(index)))
            .ToArray();
        _boardInfo = new BoardInfo(Tiles.Cast<ITileInfo>().ToArray());

        SinglePlayerLoadMap(BoardBlueprint.ChessTeam);
        _multiplayerGameService.RequestPlayMove += MultiplayerGameServiceOnRequestPlayMove;
    }

    public TileInfoViewModel SelectedTileInfo
    {
        get => _selectedTileInfo;
        private set
        {
            _selectedTileInfo.IsSelected = false;
            SetProperty(ref _selectedTileInfo, value);
            RaisePropertyChanged(nameof(TileInfoInfo));
            value.IsSelected = true;
        }
    }

    public TileInfoViewModel MouseOnTileInfo
    {
        get => _mouseOnTileInfo;
        private set
        {
            _mouseOnTileInfo.IsMouseOver = false;
            SetProperty(ref _mouseOnTileInfo, value);

            if (_selectedTileInfo.Equals(TileInfoViewModel.None))
            {
                ClearPossibleActions();
                SetPossibleActions(value, false);
            }

            RaisePropertyChanged(nameof(TileInfoInfo));
            value.IsMouseOver = true;
        }
    }

    public TileInfoViewModel TileInfoInfo
    {
        get => SelectedTileInfo.Equals(TileInfoViewModel.None)
            ? SelectedTileInfo
            : MouseOnTileInfo;
    }

    public int BoardWidth => Constants.BoardLength;
    public TileInfoViewModel[] Tiles { get; }

    public RelayCommand SurrenderCommand { get; }
    public AsyncRelayCommand<TileInfoViewModel> PlayTileCommand { get; }
    public RelayCommand<TileInfoViewModel> MouseEnterCommand { get; }
    public RelayCommand<TileInfoViewModel> MouseExitCommand { get; }

    public event EventHandler? RequestSwitchToMenu;
    public event EventHandler? RequestSwitchToGame;

    public void SinglePlayerLoadMap(BoardBlueprint map)
    {
        _boardLoader.LoadBoardExtendedFor2Players(_boardInfo, map);
        var board = _boardInfo.Select(x => new Figure(x.Figure.Owner.PlayerColor, x.Figure.IsKing, x.Figure.TypeInfo.FigureId))
            .ToArray();
        
        var whitePlayer = new ControlledPlayerInfo(PlayerColor.White, "Red player");
        var blackPlayer = new AiControlledPlayer(PlayerColor.Black, board, RequestPlayMove);
        
        _gameService.StartGame(
            whitePlayer,
            blackPlayer,
            map.StartingPlayerColor, board);
        
        RequestSwitchToGame?.Invoke(this, EventArgs.Empty);
    }

    public void MultiplayerLoadMap(
        MultiplayerGameType gameType,
        string? gameId,
        IOnlinePlayerInfo whitePlayer,
        IOnlinePlayerInfo blackPlayer,
        BoardBlueprint map,
        bool hasTimer)
    {
        if (hasTimer)
        {
            whitePlayer.SetTimer(new ChessTimer(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10)));
            blackPlayer.SetTimer(new ChessTimer(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10)));
        }

        whitePlayer.SetGameService(_multiplayerGameService);
        blackPlayer.SetGameService(_multiplayerGameService);
        
        _boardLoader.LoadBoard(_boardInfo, map);

        _multiplayerGameService.StartGame(gameType, gameId);
        _gameService.StartGame(
            whitePlayer, blackPlayer,
            map.StartingPlayerColor, map.Figures);
        
        if (_gameService.CurrentPlayerInfo is IAutomaticallyControlledPlayerInfo automaticallyControlledPlayer)
            _ = automaticallyControlledPlayer.HandleAutomaticTurnAsync();

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

    private async Task PlayTile(TileInfoViewModel? clickedTile)
    {
        ArgumentNullException.ThrowIfNull(clickedTile);
        if (clickedTile.PossibleAction.FigureActionType != FigureActionType.None)
        {
            _gameService.EndTurn();
            
            await _multiplayerGameService.PlayedMoveAsync(
                SelectedTileInfo.Position,
                clickedTile.Position,
                _gameService.CurrentPlayerInfo.Timer.LastTurnElapsedTime,
                CancellationToken.None);

            FigureActionExecutor.ExecuteFigureAction(_gameService.Board, clickedTile.PossibleAction, OnEvent);
            ClearPossibleActions();

            _soundService.PlaySoundEffect(SoundEffectType.ChessFigure);
            SelectedTileInfo = TileInfoViewModel.None;
            _gameService.StartTurn();
            if (_gameService.CurrentPlayerInfo is IAutomaticallyControlledPlayerInfo automaticallyControlledPlayer)
                _ = automaticallyControlledPlayer.HandleAutomaticTurnAsync();
        }
        else if (clickedTile.Figure.Owner.PlayerColor == _gameService.CurrentPlayerInfo.PlayerColor)
        {
            SelectedTileInfo = clickedTile;
            ClearPossibleActions();
            SetPossibleActions(clickedTile, false);
        }
        else
        {
            SelectedTileInfo = TileInfoViewModel.None;
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

    private void SetPossibleActions(TileInfoViewModel clickedTileInfo, bool automatic)
    {
        if (!_gameService.GameRunning)
            return;

        if (_gameService.CurrentPlayerInfo is IAutomaticallyControlledPlayerInfo && !automatic)
            return;

        if (_gameService.CurrentPlayerInfo.PlayerColor != clickedTileInfo.Figure.Owner.PlayerColor)
            return;

        using var possibleActions = FigureActionsResolver.GetPossibleActions(clickedTileInfo.Position, _gameService.Board);
        foreach (var possibleAction in possibleActions.Span)
        {
            if (possibleAction.FigureActionType is FigureActionType.PossibleAttack or FigureActionType.PossibleSpecial)
                continue;
            
            Tiles[possibleAction.TargetPosition.GetIndex()].PossibleAction = possibleAction;
        }
    }

    private void MouseEnterTile(TileInfoViewModel? tile)
    {
        ArgumentNullException.ThrowIfNull(tile);
        MouseOnTileInfo = tile;
    }

    private void MouseExitTile(TileInfoViewModel? tile)
    {
        ArgumentNullException.ThrowIfNull(tile);
        if (MouseOnTileInfo == tile)
        {
            MouseOnTileInfo = TileInfoViewModel.None;
        }
    }

    public void OnActivation()
    {
    }

    public void OnDeactivation()
    {
        SelectedTileInfo = TileInfoViewModel.None;
        ClearPossibleActions();
    }

    private void RequestPlayMove(Position from, Position to, TimeSpan turnTimeSpent)
    {
        SelectedTileInfo = TileInfoViewModel.None;
        ClearPossibleActions();

        switch (from.GetIndex())
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

        var fromTile = Tiles[from.GetIndex()];
        SelectedTileInfo = fromTile;
        SetPossibleActions(fromTile, true);

        _gameService.EndTurn(turnTimeSpent);
        var toTile = Tiles[to.GetIndex()];

        FigureActionExecutor.ExecuteFigureAction(_gameService.Board, toTile.PossibleAction, OnEvent);
        _soundService.PlaySoundEffect(SoundEffectType.ChessFigure);
        SelectedTileInfo = TileInfoViewModel.None;
        _gameService.StartTurn();
        ClearPossibleActions();
    }

    private void MultiplayerGameServiceOnRequestPlayMove(object? sender, (Position from, Position to, TimeSpan turnTimeSpent) e)
    {
        RequestPlayMove(e.from, e.to, e.turnTimeSpent);
    }

    private void OnEvent(BoardEvent boardEvent, Span<Figure> board)
    {
        var sourceIndex = boardEvent.SourcePosition.GetIndex();
        var targetIndex = boardEvent.TargetPosition.GetIndex();
        
        switch (boardEvent.EventType)
        {
            case BoardEventType.Attacked:
                break;
            case BoardEventType.ChangedFigure:
            case BoardEventType.ChangedOwner:
            case BoardEventType.CreatedFigure:
                Tiles[targetIndex].Figure = _figureCreator.CreateFigure(board[targetIndex]);
                Tiles[targetIndex].OnCreated();
                break;
            case BoardEventType.Died:
                Tiles[targetIndex].Figure = _figureCreator.CreateFigure(board[targetIndex]);
                Tiles[targetIndex].OnDied();
                break;
            case BoardEventType.Moved:
                Tiles[sourceIndex].Figure = _figureCreator.CreateFigure(board[sourceIndex]);
                Tiles[sourceIndex].OnMovedFrom();
                Tiles[targetIndex].Figure = _figureCreator.CreateFigure(board[targetIndex]);
                Tiles[targetIndex].OnMovedTo();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
  