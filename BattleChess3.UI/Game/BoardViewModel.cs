using BattleChess3.Game;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Helpers;
using BattleChess3.Game.Players;
using BattleChess3.Maps;
using BattleChess3.Multiplayer;
using BattleChess3.UI.Services;
using BattleChess3.UI.Shared;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Game;

public sealed class BoardViewModel : ViewModelBase
{
    private readonly IGameService _gameService;
    private readonly IMapLoader _mapLoader;
    private readonly IMultiplayerGameService _multiplayerGameService;
    private readonly ISoundService _soundService;

    private TileViewModel _mouseOnTile = NoneTileViewModel.Instance;
    private TileViewModel _selectedTile = NoneTileViewModel.Instance;

    public BoardViewModel(
        IGameService gameService,
        IMapLoader mapLoader,
        IMultiplayerGameService multiplayerGameService,
        ISoundService soundService)
    {
        _gameService = gameService;
        _mapLoader = mapLoader;
        _multiplayerGameService = multiplayerGameService;
        _soundService = soundService;

        SurrenderCommand = new RelayCommand(Surrender);
        PlayTileCommand = new RelayCommand<TileViewModel>(PlayTile);
        MouseEnterCommand = new RelayCommand<TileViewModel>(MouseEnterTile);
        MouseExitCommand = new RelayCommand<TileViewModel>(MouseExitTile);

        Tiles = Enumerable.Range(0, Constants.FullBoardTilesCount)
            .Select<int, TileViewModel>(index => new TileViewModel(Position.FromIndex(index)))
            .ToArray();
        Board = new Board(Tiles.Cast<ITile>().ToArray());
        
        SinglePlayerLoadMap(MapBlueprint.ChessTeam);
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
            
            if (_selectedTile.Equals(NoneTileViewModel.Instance))
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
        get => SelectedTile is not NoneTileViewModel
            ? SelectedTile
            : MouseOnTile;
    }

    public int BoardWidth => Constants.BoardLength;
    public IBoard Board { get; }
    public TileViewModel[] Tiles { get; }
    
    public RelayCommand SurrenderCommand { get; }
    public RelayCommand<TileViewModel> PlayTileCommand { get; }
    public RelayCommand<TileViewModel> MouseEnterCommand { get; }
    public RelayCommand<TileViewModel> MouseExitCommand { get; }

    public event EventHandler? RequestSwitchToMenu;
    public event EventHandler? RequestSwitchToGame;

    public void SinglePlayerLoadMap(MapBlueprint map)
    {
        _gameService.StartGame(
            new PlayerInfo(Player.White, "Red player", null, null),
            new PlayerInfo(Player.Black, "Blue player", null, null),
            map.StartingPlayer, false, false);
        _mapLoader.LoadMapExtendedFor2Players(Board, map);
        RequestSwitchToGame?.Invoke(this, EventArgs.Empty);
    }

    public void MultiplayerLoadMap(
        MultiplayerGameType gameType, 
        string? gameId, 
        PlayerInfo player1,
        PlayerInfo player2,
        MapBlueprint map, 
        bool hasTimer)
    {
        _gameService.StartGame(
            player1, player2,
            map.StartingPlayer, true, hasTimer);
        _mapLoader.LoadMap(Board, map);
        RequestSwitchToGame?.Invoke(this, EventArgs.Empty);
        
        _multiplayerGameService.StartGame(gameType, gameId);
        if (_gameService.IsWaitingForMove)
        {
            _multiplayerGameService.HandleHisTurnAsync();
        }
    }

    private void Surrender()
    {
        if (_gameService.IsMultiplayer &&
            (_gameService.CanMove || _gameService.IsWaitingForMove))
        {
            _soundService.PlaySoundEffect(SoundEffectType.Button);
            _gameService.Surrender();
        }
        else
        {
            RequestSwitchToMenu?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PlayTile(TileViewModel? clickedTile)
    {
        ArgumentNullException.ThrowIfNull(clickedTile);
        if (clickedTile.PossibleAction.ActionType != FigureActionTypes.None)
        {
            var timeSpent = _gameService.EndTurn();
            _multiplayerGameService.PlayedMoveAsync(SelectedTile.RelativePosition, clickedTile.RelativePosition, timeSpent);
            clickedTile.PossibleAction.Action.Invoke();
            _soundService.PlaySoundEffect(SoundEffectType.ChessFigure);
            SelectedTile = NoneTileViewModel.Instance;
            _gameService.NextTurn();
            if (_gameService.IsWaitingForMove)
            {
                _multiplayerGameService.HandleHisTurnAsync();
            }
        }
        else if (clickedTile.Figure.Owner.Equals(_gameService.CurrentPlayerInfo))
        {
            SelectedTile = clickedTile;
        }
        else
        {
            SelectedTile = NoneTileViewModel.Instance;
        }

        ClearPossibleActions();
        SetPossibleActions(clickedTile, false);
    }


    private void ClearPossibleActions()
    {
        foreach (var tile in Tiles)
        {
            tile.PossibleAction = FigureAction.None;
        }
    }

    private void SetPossibleActions(TileViewModel clickedTile, bool remote)
    {
        if (!_gameService.CanMove && !remote)
            return;
        
        if (!_gameService.CurrentPlayerInfo.Equals(clickedTile.Figure.Owner))
            return;
        
        var relativeBoard = GetPlayerRelativeBoard(clickedTile.Figure.Owner.Player, Tiles);
        var relativeClickedTile = clickedTile.GetRelativeTile(clickedTile.Figure.Owner.Player);
        var possibleActions = clickedTile.Figure.GetPossibleActions(relativeClickedTile, relativeBoard);

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
            povBoard[PlayerPositionHelper.GetRelativePosition(player, position).GetIndex()] = absoluteBoard[position.GetIndex()];
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
            MouseOnTile = NoneTileViewModel.Instance;
        }
    }

    public void OnActivation()
    {
    }

    public void OnDeactivation()
    {
        SelectedTile = NoneTileViewModel.Instance;
        ClearPossibleActions();
    }

    private void MultiplayerGameServiceOnRequestPlayMove(object? sender, (Position from, Position to, TimeSpan turnTimeSpent) e)
    {
        SelectedTile = NoneTileViewModel.Instance;
        ClearPossibleActions();

        switch (e.from.GetIndex())
        {
            case IMultiplayerGameService.NotRespondingMessage:
                _gameService.PlayerWin(_gameService.GetPlayerInfo(Player.White), WinType.NotResponding, true);
                return;
            case IMultiplayerGameService.NotRespondingLostMessage:
                _gameService.PlayerWin(_gameService.GetPlayerInfo(Player.Black), WinType.NotResponding, false);
                return;
            case IMultiplayerGameService.OutOfTimeMessage:
                _gameService.PlayerWin(_gameService.GetPlayerInfo(Player.White), WinType.OutOfTime, false);
                return;
            case IMultiplayerGameService.SurrenderMessage:
                _gameService.PlayerWin(_gameService.GetPlayerInfo(Player.White), WinType.Surrender, false);
                return;
        }

        var fromTile = Tiles[e.from.GetIndex()];
        SelectedTile = fromTile;
        SetPossibleActions(fromTile, true);
        
        _gameService.EndTurn(e.turnTimeSpent);
        var toTile = Tiles[e.to.GetIndex()];
        toTile.PossibleAction.Action.Invoke();
        _soundService.PlaySoundEffect(SoundEffectType.ChessFigure);
        SelectedTile = NoneTileViewModel.Instance;
        _gameService.NextTurn();
        if (_gameService.IsWaitingForMove)
        {
            _multiplayerGameService.HandleHisTurnAsync();
        }
        ClearPossibleActions();
    }
}