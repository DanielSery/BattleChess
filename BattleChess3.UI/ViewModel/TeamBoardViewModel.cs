using BattleChess3.CrossFireFigures;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;
using BattleChess3.Maps;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BattleChess3.UI.ViewModel;

public class TeamBoardViewModel : ViewModelBase
{
    private const int BasePoints = 85;

    private bool _hasKing;
    private int _totalPoints;
    
    private readonly IFigureCreator _figureCreator;

    public TeamBoardViewModel(
        IFigureCreator figureCreator,
        MapsViewModel maps,
        IMapLoader mapLoader)
    {
        _figureCreator = figureCreator;
        
        Tiles = Enumerable.Range(0, IBoard.Length * 2)
            .Select<int, TileViewModel>(index => new TileViewModel(Position.FromIndex(index)))
            .ToArray();
        Board = new Board(Tiles.Cast<ITile>().ToArray());
        
        for (var i = 0; i < Board.Count; i++)
        {
            Board[i].Figure = new Figure(Player.Neutral, CrossFireFigureGroup.Empty, false);
        }
        
        mapLoader.LoadTeamMap(Board, maps.SelectedMap);
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
        private set => Set(ref _totalPoints, value);
    }

    public bool CanSave => PositivePoints && HasKing;

    public bool HasKing
    {
        get => _hasKing;
        private set => Set(ref _hasKing, value);
    }
    
    public RelayCommand<TileViewModel> MakeUnitKingCommand { get; }
    public event EventHandler<string>? RequestSavePreview;

    public void CreateFigure(ITile tile, FigureIdentifier figureIdentifier)
    {
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = _figureCreator.CreateFigure(figureIdentifier);

        TotalPoints = Board.Sum(x => x.Figure.FigureValue);
        HasKing = Board.Any(x => x.Figure.IsKing);
        RaisePropertyChanged(nameof(PointsLeft));
        RaisePropertyChanged(nameof(PositivePoints));
        RaisePropertyChanged(nameof(CanSave));
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

    public void RequestSave(string identifier)
    {
        RequestSavePreview?.Invoke(this, identifier);
    }
}