using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;
using BattleChess3.Maps;
using BattleChess3.Multiplayer;
using BattleChess3.UI.Shared;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Game;

public sealed class BoardViewModel : ViewModelBase
{
    private readonly IPlayerService _playerService;
    private readonly IMapLoader _mapLoader;
    private readonly IMultiplayerGameService _multiplayerGameService;

    private TileViewModel _mouseOnTile = NoneTileViewModel.Instance;
    private TileViewModel _selectedTile = NoneTileViewModel.Instance;

    public BoardViewModel(
        IPlayerService playerService,
        IMapLoader mapLoader,
        IMultiplayerGameService multiplayerGameService)
    {
        _playerService = playerService;
        _mapLoader = mapLoader;
        _multiplayerGameService = multiplayerGameService;

        SurrenderCommand = new RelayCommand(Surrender);
        PlayTileCommand = new RelayCommand<TileViewModel>(PlayTile);
        MouseEnterCommand = new RelayCommand<TileViewModel>(MouseEnterTile);
        MouseExitCommand = new RelayCommand<TileViewModel>(MouseExitTile);

        Tiles = Enumerable.Range(0, IBoard.TilesCount)
            .Select<int, TileViewModel>(index => new TileViewModel(Position.FromIndex(index)))
            .ToArray();
        Board = new Board(Tiles.Cast<ITile>().ToArray());
        
        SinglePlayerLoadMap(MapBlueprint.EmptyTeam);
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

    public int BoardWidth
    {
        get => IBoard.Length;
    }

    public IBoard Board { get; }
    public TileViewModel[] Tiles { get; }
    
    public RelayCommand SurrenderCommand { get; }
    public RelayCommand<TileViewModel> PlayTileCommand { get; }
    public RelayCommand<TileViewModel> MouseEnterCommand { get; }
    public RelayCommand<TileViewModel> MouseExitCommand { get; }

    public event EventHandler? RequestSwitchToMenu;
    public event EventHandler? RequestSwitchToGame;

    public void ClearSelectedTile()
    {
        SelectedTile = NoneTileViewModel.Instance;
        ClearPossibleActions();
    }

    public void SinglePlayerLoadMap(MapBlueprint map)
    {
        _playerService.InitializePlayers(
            new Player(null, "Red player", null, 1),
            new Player(null, "Blue player", null, 2),
            map.StartingPlayer, false, false);
        _mapLoader.LoadMapExtendedFor2Players(Board, map);
        RequestSwitchToGame?.Invoke(this, EventArgs.Empty);
    }

    public void MultiplayerLoadMap(
        MultiplayerGameType gameType, 
        string? gameId, 
        Player player1,
        Player player2,
        MapBlueprint map, 
        bool hasTimer)
    {
        _playerService.InitializePlayers(
            player1, player2,
            map.StartingPlayer, true, hasTimer);
        _mapLoader.LoadMap(Board, map);
        RequestSwitchToGame?.Invoke(this, EventArgs.Empty);
        
        _multiplayerGameService.StartGame(gameType, gameId);
        if (_playerService.IsWaitingForMove)
        {
            _multiplayerGameService.HandleHisTurnAsync();
        }
    }

    private void Surrender()
    {
        if (_playerService.IsMultiplayer &&
            (_playerService.CanMove || _playerService.IsWaitingForMove))
        {
            _playerService.Surrender();
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
            var timeSpent = _playerService.EndTurn();
            _multiplayerGameService.PlayedMoveAsync(SelectedTile.Position, clickedTile.Position, timeSpent);
            clickedTile.PossibleAction.Action.Invoke();
            SelectedTile = NoneTileViewModel.Instance;
            _playerService.NextTurn();
            if (_playerService.IsWaitingForMove)
            {
                _multiplayerGameService.HandleHisTurnAsync();
            }
        }
        else if (clickedTile.Figure.Owner.Equals(_playerService.CurrentPlayer))
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
        if (!_playerService.CanMove && !remote)
            return;
        
        if (!_playerService.CurrentPlayer.Equals(clickedTile.Figure.Owner))
            return;
        
        var povBoard = GetPlayerPOVBoard(clickedTile.Figure.Owner, Tiles);
        var povClickedTile = clickedTile.GetPovTile(clickedTile.Figure.Owner);
        var possibleActions = clickedTile.Figure.GetPossibleActions(povClickedTile, povBoard);
        
        foreach (var possibleAction in possibleActions)
        {
            Tiles[possibleAction.TargetPosition.Index].PossibleAction = possibleAction;
        }
    }

    private static IBoard GetPlayerPOVBoard(Player player, IReadOnlyList<ITile> board)
    {
        var povBoard = new ITile[IBoard.TilesCount];
        var absoluteBoard = board.Select(x => x.GetPovTile(player)).ToArray();

        for (var i = 0; i < IBoard.Length; i++)
        for (var j = 0; j < IBoard.Length; j++)
        {
            var position = new Position(j, i);
            povBoard[position.GetPlayerPOVPosition(player).Index] = absoluteBoard[position.Index];
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

    private void MultiplayerGameServiceOnRequestPlayMove(object? sender, (Position from, Position to, TimeSpan turnTimeSpent) e)
    {
        SelectedTile = NoneTileViewModel.Instance;
        ClearPossibleActions();

        switch (e.from.Index)
        {
            case IMultiplayerGameService.NotRespondingMessage:
                _playerService.PlayerWin(_playerService.GetPlayer(1), WinType.NotResponding, true);
                return;
            case IMultiplayerGameService.NotRespondingLostMessage:
                _playerService.PlayerWin(_playerService.GetPlayer(2), WinType.NotResponding, false);
                return;
            case IMultiplayerGameService.OutOfTimeMessage:
                _playerService.PlayerWin(_playerService.GetPlayer(1), WinType.OutOfTime, false);
                return;
            case IMultiplayerGameService.SurrenderMessage:
                _playerService.PlayerWin(_playerService.GetPlayer(1), WinType.Surrender, false);
                return;
        }

        var fromTile = Tiles[e.from.Index];
        SelectedTile = fromTile;
        SetPossibleActions(fromTile, true);
        
        _playerService.EndTurn(e.turnTimeSpent);
        var toTile = Tiles[e.to.Index];
        toTile.PossibleAction.Action.Invoke();
        SelectedTile = NoneTileViewModel.Instance;
        _playerService.NextTurn();
        if (_playerService.IsWaitingForMove)
        {
            _multiplayerGameService.HandleHisTurnAsync();
        }
        ClearPossibleActions();
    }
}