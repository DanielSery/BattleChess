using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;
using BattleChess3.Maps;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class TeamBoardViewModel : ViewModelBase
{
    private const int BasePoints = 84;

    private bool _hasKing;
    private int _totalPoints;
    
    private readonly IFigureCreator _figureCreator;
    private readonly MapsViewModel _maps;
    private readonly IMapLoader _mapLoader;

    public TeamBoardViewModel(
        IFigureCreator figureCreator,
        MapsViewModel maps,
        IMapLoader mapLoader)
    {
        _figureCreator = figureCreator;
        _maps = maps;
        _mapLoader = mapLoader;
        
        Tiles = Enumerable.Range(0, IBoard.Length * 2)
            .Select<int, TileViewModel>(index => new TileViewModel(Position.FromIndex(index)))
            .ToArray();
        
        Board = new Board(Tiles.Cast<ITile>().ToArray());
        mapLoader.LoadMap(Board, maps.TeamMap);
        EvaluateTeamBoard();
        
        MakeUnitKingCommand = new RelayCommand<TileViewModel>(MakeUnitKing);
    }

    public int BoardWidth => IBoard.Length;
    public IBoard Board { get; }
    public TileViewModel[] Tiles { get; }

    public bool PositivePoints => PointsLeft >= 0;
    public int PointsLeft => BasePoints - TotalPoints;

    public int TotalPoints
    {
        get => _totalPoints;
        private set => SetProperty(ref _totalPoints, value);
    }

    public bool CanSave => PositivePoints && HasKing;

    public bool HasKing
    {
        get => _hasKing;
        private set => SetProperty(ref _hasKing, value);
    }
    
    public RelayCommand<TileViewModel> MakeUnitKingCommand { get; }

    public void CreateFigure(ITile tile, FigureIdentifier figureIdentifier)
    {
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = _figureCreator.CreateFigure(figureIdentifier);
        EvaluateTeamBoard();
    }

    private void EvaluateTeamBoard()
    {
        TotalPoints = Board.Sum(x => x.Figure.FigureValue);
        HasKing = Board.Any(x => x.Figure.IsKing);
        RaisePropertyChanged(nameof(PointsLeft));
        RaisePropertyChanged(nameof(PositivePoints));
        RaisePropertyChanged(nameof(CanSave));
    }

    public void SaveMap()
    {
        _maps.SaveMap(Tiles);
    }

    public void Discard()
    {
        _mapLoader.LoadMap(Board, _maps.TeamMap);
    }

    private void MakeUnitKing(TileViewModel? tile)
    {
        ArgumentNullException.ThrowIfNull(tile);

        if (tile.Figure.Owner.Equals(Player.Neutral))
            return;

        if (tile.Figure.IsKing)
        {
            var demotedFigureId = tile.Figure.Type.FigureId;
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
            
            var demotedFigureId = checkedTile.Figure.Type.FigureId;
            checkedTile.Figure.Owner.Figures.Remove(checkedTile.Figure);
            checkedTile.Figure = _figureCreator.CreateFigure(new FigureIdentifier(owner.Id, demotedFigureId, false));
        }
        
        var upgradedFigureId = tile.Figure.Type.FigureId;
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = _figureCreator.CreateFigure(new FigureIdentifier(owner.Id, upgradedFigureId, true));
        
        HasKing = true;
        RaisePropertyChanged(nameof(CanSave));
    }
}