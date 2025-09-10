using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.Game.GameBoard;
using CrownsGuard.Maps;
using CrownsGuard.Maps.BoardBlueprints;
using CrownsGuard.Maps.Figures;
using CrownsGuard.Multiplayer;
using CrownsGuard.Multiplayer.Players;
using CommunityToolkit.Mvvm.Input;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.UI.Services;
using CrownsGuard.UI.Shared;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Editor;

public class TeamBoardViewModel : ViewModelBase
{
    private const int BasePoints = 84;

    private bool _hasKing;
    private int _totalPoints;
    
    private readonly IFigureCreator _figureCreator;
    private readonly MapsViewModel _maps;
    private readonly IBoardLoader _boardLoader;
    private readonly IMultiplayerPlayerService _playerService;
    private readonly ISoundService _soundService;

    public TeamBoardViewModel(
        IFigureCreator figureCreator,
        MapsViewModel maps,
        IBoardLoader boardLoader, 
        IMultiplayerPlayerService playerService,
        ISoundService soundService)
    {
        _figureCreator = figureCreator;
        _maps = maps;
        _boardLoader = boardLoader;
        _playerService = playerService;
        _soundService = soundService;
        
        Tiles = Enumerable.Range(0, Constants.BoardLength * 2)
            .Select<int, TileViewModel>(index => new TileViewModel(Position.FromIndex(index)))
            .ToArray();
        
        Board = new Board(Tiles.Cast<ITile>().ToArray());
        boardLoader.LoadTeamBoard(Board, maps.TeamMap);
        EvaluateTeamBoard();
        
        MakeUnitKingCommand = new RelayCommand<TileViewModel>(MakeUnitKing);
        
        _playerService.LoggedInPlayerChanged += PlayerServiceOnLoggedInPlayerChanged;
    }

    public int BoardWidth => Constants.BoardLength;
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

    public void CreateFigure(ITile tile, Figure figure)
    {
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = _figureCreator.CreateFigure(figure);
        _soundService.PlaySoundEffect(SoundEffectType.Button);
        EvaluateTeamBoard();
    }

    private void EvaluateTeamBoard()
    {
        TotalPoints = Board.Sum(x => x.Figure.Type.FigureValue);
        HasKing = Board.Any(x => x.Figure.IsKing);
        RaisePropertyChanged(nameof(PointsLeft));
        RaisePropertyChanged(nameof(PositivePoints));
        RaisePropertyChanged(nameof(CanSave));
    }

    public void SaveMap()
    {
        _maps.SaveMap(Tiles);
    }

    public BoardBlueprint GetMapBlueprint()
    {
        return new BoardBlueprint
        {
            Figures = Tiles.Select(x => new Figure(x.Figure.Owner.Player, x.Figure.IsKing, x.Figure.Type.FigureId)).ToArray(),
        };
    }

    private void PlayerServiceOnLoggedInPlayerChanged(object? sender, EventArgs e)
    {
        var loggedInPlayer = _playerService.LoggedInPlayer;
        if (loggedInPlayer is null)
            return;
        
        var mapBlueprint = GetMapBlueprint(loggedInPlayer.Map);
        _boardLoader.LoadTeamBoard(Board, mapBlueprint);
    }
    
    private static BoardBlueprint GetMapBlueprint(int[] map)
    {
        var figures = new Figure[16];
        for (var i = 0; i < figures.Length; i++)
        {
            var value = map[i];
            var player = (Player)(value & 0xFF);       // lowest 8 bits
            var isKing = ((value >> 8) & 1) != 0;    // next bit
            var figureType = (FigureId)((value >> 9) & 0xFFFF); // next 16 bits
            figures[i] = new Figure(player, isKing, figureType);
        }

        return new BoardBlueprint
        {
            Figures = figures,
            StartingPlayer = Player.White,
        };
    }

    public void Discard()
    {
        _boardLoader.LoadTeamBoard(Board, _maps.TeamMap);
    }

    private void MakeUnitKing(TileViewModel? tile)
    {
        ArgumentNullException.ThrowIfNull(tile);

        if (tile.Figure.Owner.Equals(NeutralFigureOwner.Instance))
            return;

        _soundService.PlaySoundEffect(SoundEffectType.Button);
        if (tile.Figure.IsKing)
        {
            var demotedFigureId = tile.Figure.Type.FigureId;
            tile.Figure.Owner.Figures.Remove(tile.Figure);
            tile.Figure = _figureCreator.CreateFigure(new Figure(tile.Figure.Owner.Player, false, demotedFigureId));
            
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
            checkedTile.Figure = _figureCreator.CreateFigure(new Figure(owner.Player, false, demotedFigureId));
        }
        
        var upgradedFigureId = tile.Figure.Type.FigureId;
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = _figureCreator.CreateFigure(new Figure(owner.Player, true, upgradedFigureId));
        
        HasKing = true;
        RaisePropertyChanged(nameof(CanSave));
    }
}