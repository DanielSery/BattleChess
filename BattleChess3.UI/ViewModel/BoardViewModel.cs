﻿using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.Core.Resources;
using BattleChess3.UI.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BattleChess3.UI.ViewModel;

public sealed class BoardViewModel : ViewModelBase
{
    private readonly IFigureService _figureService;
    private readonly IPlayerService _playerService;

    private ITileViewModel _selectedTile = NoneTileViewModel.Instance;
    public ITileViewModel SelectedTile
    {
        get => _selectedTile;
        private set
        {
            _selectedTile.IsSelected = false;
            Set(ref _selectedTile, value);
            RaisePropertyChanged(nameof(InfoTile));
            value.IsSelected = true;
        }
    }

    private ITileViewModel _mouseOnTile = NoneTileViewModel.Instance;
    public ITileViewModel MouseOnTile
    {
        get => _mouseOnTile;
        private set
        {
            _mouseOnTile.IsMouseOver = false;
            Set(ref _mouseOnTile, value);
            RaisePropertyChanged(nameof(InfoTile));
            value.IsMouseOver = true;
        }
    }

    public ITileViewModel InfoTile =>
        SelectedTile is not NoneTileViewModel
            ? SelectedTile
            : MouseOnTile;

    private int _boardWidth = 8;
    public int BoardWidth
    {
        get => _boardWidth;
        set => Set(ref _boardWidth, value);
    }

    public ITileViewModel[] Board { get; } = Enumerable.Range(0, Constants.BoardSize)
        .Select<int, ITileViewModel>(position => new TileViewModel(position))
        .ToArray();
    
    public RelayCommand<ITileViewModel> ClickedCommand { get; }
    public RelayCommand<ITileViewModel> MouseEnterCommand { get; }
    public RelayCommand<ITileViewModel> MouseExitCommand { get; }


    public event EventHandler<Position>? RequestClickTile;
    public event EventHandler<MapBlueprint>? RequestLoadMap;


    public BoardViewModel(
        IFigureService figureService,
        IPlayerService playerService)
    {
        _figureService = figureService;
        _playerService = playerService;

        ClickedCommand = new RelayCommand<ITileViewModel>(ClickedAtTile);
        MouseEnterCommand = new RelayCommand<ITileViewModel>(MouseEnterTile);
        MouseExitCommand = new RelayCommand<ITileViewModel>(MouseExitTile);
    }
    
    public void ManualLoadMap(MapBlueprint map)
    {
        AutomaticLoadMap(map);
        RequestLoadMap?.Invoke(this, map);
    }

    public void AutomaticLoadMap(MapBlueprint map)
    {
        AutomaticClickAtTile(NoneTileViewModel.Instance);
        _playerService.InitializePlayers(map.StartingPlayer);

        for (var i = 0; i < Constants.BoardSize; i++)
            CreateFigure(Board[i], map.Figures[i]);
    }

    public void CreateFigure(ITileViewModel tile, FigureBlueprint figureBlueprint)
    {
        var figureType = _figureService.GetFigureFromName(figureBlueprint.UnitName);
        var player = _playerService.GetPlayer(figureBlueprint.PlayerId);
        var figure = new Figure(player, figureType);
        player.Figures.Add(figure);
        tile.Figure = figure;
    }

    private void ClickedAtTile(ITileViewModel clickedTile)
    {
        AutomaticClickAtTile(clickedTile);
        RequestClickTile?.Invoke(this, clickedTile.Position);
    }

    public void AutomaticClickAtTile(ITileViewModel clickedTile)
    {
        if (clickedTile.PossibleAction.ActionType != FigureActionTypes.None)
        {
            clickedTile.PossibleAction.Action.Invoke();
            SelectedTile = NoneTileViewModel.Instance;
            _playerService.NextTurn();
        }
        else if (clickedTile.Figure.Owner.Equals(_playerService.CurrentPlayer))
        {
            SelectedTile = clickedTile;
        }
        else
        {
            SelectedTile = NoneTileViewModel.Instance;
        }

        SetPossibleActions(clickedTile);
    }

    private void SetPossibleActions(ITileViewModel clickedTile)
    {
        foreach (var tile in Board)
        {
            tile.PossibleAction = FigureAction.None;
        }

        if (_playerService.CurrentPlayer != SelectedTile.Figure.Owner)
            return;

        SetPossibleActions((ITile)clickedTile);
    }

    private void SetPossibleActions(ITile clickedTile)
    {
        var povBoard = GetPlayerPOVBoard(_playerService.CurrentPlayer, Board);
        var povClickedTile = clickedTile.GetPovTile(_playerService.CurrentPlayer);
        foreach (var tile in povBoard)
        {
            if (clickedTile.Position == tile.Position)
                continue;

            var targetTile = tile.GetPovTile(_playerService.CurrentPlayer);
            Board[tile.Position].PossibleAction = clickedTile.Figure.GetPossibleAction(povClickedTile, targetTile, povBoard);
        }
    }

    private static ITile[] GetPlayerPOVBoard(Player player, IReadOnlyList<ITile> board)
    {
        var povBoard = new ITile[Constants.BoardSize];

        for (var i = 0; i < Constants.BoardLength; i++)
        for (var j = 0; j < Constants.BoardLength; j++)
        {
            var position = new Position(j, i);
            povBoard[position.GetPlayerPOVPosition(player)] = board[position];
        }

        return povBoard;
    }

    private void MouseEnterTile(ITileViewModel tile)
        => MouseOnTile = tile;

    private void MouseExitTile(ITileViewModel tile)
    {
        if (MouseOnTile == tile)
        {
            MouseOnTile = NoneTileViewModel.Instance;
        }
    }
}
