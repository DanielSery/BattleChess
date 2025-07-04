using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NUnit.Framework;

namespace BattleChess3.UI.ViewModel;

public sealed class EditorViewModel : ViewModelBase, IDisposable
{
    private readonly IFigureService _figureService;
    private readonly IFigureCreator _figureCreator;

    private FigureTypeViewModel[] _figures = [];
    private FigureTypeViewModel _tileInfo = new FigureTypeViewModel(Figure.None);
    private bool _tileInfoFocused;
    private int _pointsLeft;
    private bool _hasKing;

    public EditorViewModel(
        IFigureService figureService,
        IFigureCreator figureCreator)
    {
        _figureService = figureService;
        _figureService.FigureGroupsChanged += OnFigureGroupsChanged;
        Figures = _figureService.GetFigureGroups()
            .SelectMany(x => x.FigureTypes)
            .Select(x => new FigureTypeViewModel(x))
            .ToArray();

        _figureCreator = figureCreator;

        FigureGotFocusCommand = new RelayCommand<FigureTypeViewModel>(GotFocus);
        FigureLostFocusCommand = new RelayCommand<FigureTypeViewModel>(LostFocus);
        FigureMouseEnterCommand = new RelayCommand<FigureTypeViewModel>(MouseEnterTile);
        FigureMouseExitCommand = new RelayCommand<FigureTypeViewModel>(MouseExitTile);
        MakeUnitKingCommand = new RelayCommand<TileViewModel>(MakeUnitKing);

        Tiles = Enumerable.Range(0, IBoard.Length * 2)
            .Select<int, TileViewModel>(index => new TileViewModel(Position.FromIndex(index)))
            .ToArray();
        Board = new Board(Tiles.Cast<ITile>().ToArray());
        
        for (var i = 0; i < Board.Count; i++)
        {
            Board[i].Figure = new Figure(Player.Neutral, _figureService.GetFigureByUniqueUnitId(0), false);
        }
    }

    public int BoardWidth
    {
        get => IBoard.Length;
    }

    public IBoard Board { get; }
    public TileViewModel[] Tiles { get; }

    public int PointsLeft
    {
        get => _pointsLeft;
        private set => Set(ref _pointsLeft, value);
    }

    public bool CanSave => PointsLeft > 0 && HasKing;

    public bool HasKing
    {
        get => _hasKing;
        private set => Set(ref _hasKing, value);
    }

    public FigureTypeViewModel[] Figures
    {
        get => _figures;
        private set => Set(ref _figures, value);
    }

    public FigureTypeViewModel TileInfo
    {
        get => _tileInfo;
        private set => Set(ref _tileInfo, value);
    }

    public RelayCommand<TileViewModel> MakeUnitKingCommand { get; }
    public RelayCommand<FigureTypeViewModel> FigureGotFocusCommand { get; }
    public RelayCommand<FigureTypeViewModel> FigureLostFocusCommand { get; }
    public RelayCommand<FigureTypeViewModel> FigureMouseEnterCommand { get; }
    public RelayCommand<FigureTypeViewModel> FigureMouseExitCommand { get; }

    private void GotFocus(FigureTypeViewModel obj)
    {
        TileInfo = obj;
        _tileInfoFocused = true;
    }

    private void LostFocus(FigureTypeViewModel obj)
    {
        TileInfo = new FigureTypeViewModel(Figure.None);
        _tileInfoFocused = false;
    }

    private void MouseExitTile(FigureTypeViewModel obj)
    {
        if (_tileInfoFocused)
            return;
        
        TileInfo = new FigureTypeViewModel(Figure.None);
    }

    private void MouseEnterTile(FigureTypeViewModel obj)
    {
        if (_tileInfoFocused)
            return;

        TileInfo = obj;
    }

    public void CreateFigure(ITile tile, FigureIdentifier figureIdentifier)
    {
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = _figureCreator.CreateFigure(figureIdentifier);
    }

    private void MakeUnitKing(TileViewModel tile)
    {
        if (tile.Figure.Owner.Equals(Player.Neutral))
            return;

        if (tile.Figure.IsKing)
        {
            var demotedFigureId = tile.Figure.Type.UniqueFigureId;
            tile.Figure.Owner.Figures.Remove(tile.Figure);
            tile.Figure = _figureCreator.CreateFigure(new FigureIdentifier(tile.Figure.Owner.Id, demotedFigureId, false));
            
            HasKing = false;
            RaisePropertyChanged(nameof(CanSave));
            return;
        }
        
        var owner = tile.Figure.Owner;
        foreach (var checkedTile in Board)
        {
            if (!checkedTile.Figure.Owner.Equals(owner) ||
                !checkedTile.Figure.IsKing) 
                continue;
            
            var demotedFigureId = checkedTile.Figure.Type.UniqueFigureId;
            checkedTile.Figure.Owner.Figures.Remove(checkedTile.Figure);
            checkedTile.Figure = _figureCreator.CreateFigure(new FigureIdentifier(owner.Id, demotedFigureId, false));
        }
        
        var upgradedFigureId = tile.Figure.Type.UniqueFigureId;
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = _figureCreator.CreateFigure(new FigureIdentifier(owner.Id, upgradedFigureId, true));
        
        HasKing = true;
        RaisePropertyChanged(nameof(CanSave));
    }

    public void Dispose()
    {
        _figureService.FigureGroupsChanged -= OnFigureGroupsChanged;
    }

    private void OnFigureGroupsChanged(object? sender, IList<IFigureGroup> groups)
    {
        Figures = groups.SelectMany(x => x.FigureTypes)
            .Select(x => new FigureTypeViewModel(x))
            .ToArray();
    }
}