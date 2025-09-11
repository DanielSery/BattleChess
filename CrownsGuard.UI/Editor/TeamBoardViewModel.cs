using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Maps.BoardBlueprints;
using CrownsGuard.Maps.Figures;
using CrownsGuard.Multiplayer.Players;
using CommunityToolkit.Mvvm.Input;
using CrownsGuard.FigureDefinitions.Utilities;
using CrownsGuard.Maps.GameBoard;
using CrownsGuard.UI.Services;
using CrownsGuard.UI.Shared;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Editor;

public class TeamBoardViewModel : ViewModelBase
{
    private const int BasePoints = 84;
    
    private readonly IBoardInfo _boardInfo;

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
            .Select<int, TileInfoViewModel>(index => new TileInfoViewModel(Position.FromIndex(index)))
            .ToArray();
        
        _boardInfo = new BoardInfo(Tiles.Cast<ITileInfo>().ToArray());
        boardLoader.LoadTeamBoard(_boardInfo, maps.TeamMap);
        EvaluateTeamBoard();
        
        MakeUnitKingCommand = new RelayCommand<TileInfoViewModel>(MakeUnitKing);
        
        _playerService.LoggedInPlayerChanged += PlayerServiceOnLoggedInPlayerChanged;
    }

    public int BoardWidth => Constants.BoardLength;
    public TileInfoViewModel[] Tiles { get; }

    public bool PositivePoints => PointsLeft >= 0;
    public int PointsLeft => BasePoints - TotalPoints;

    private int TotalPoints
    {
        get => _totalPoints;
        set => SetProperty(ref _totalPoints, value);
    }

    public bool CanSave => PositivePoints && HasKing;

    public bool HasKing
    {
        get => _hasKing;
        private set => SetProperty(ref _hasKing, value);
    }
    
    public RelayCommand<TileInfoViewModel> MakeUnitKingCommand { get; }

    public void CreateFigure(ITileInfo tileInfo, Figure figure)
    {
        tileInfo.Figure = _figureCreator.CreateFigure(figure);
        _soundService.PlaySoundEffect(SoundEffectType.Button);
        EvaluateTeamBoard();
    }

    private void EvaluateTeamBoard()
    {
        TotalPoints = _boardInfo.Sum(x => x.Figure.TypeInfo.FigureId.GetFigureValue());
        HasKing = _boardInfo.Any(x => x.Figure.IsKing);
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
            Figures = Tiles.Select(x => new Figure(x.Figure.Owner.PlayerColor, x.Figure.IsKing, x.Figure.TypeInfo.FigureId)).ToArray(),
        };
    }

    private void PlayerServiceOnLoggedInPlayerChanged(object? sender, EventArgs e)
    {
        var loggedInPlayer = _playerService.LoggedInPlayer;
        if (loggedInPlayer is null)
            return;
        
        var mapBlueprint = GetMapBlueprint(loggedInPlayer.Map);
        _boardLoader.LoadTeamBoard(_boardInfo, mapBlueprint);
    }
    
    private static BoardBlueprint GetMapBlueprint(int[] map)
    {
        var figures = new Figure[16];
        for (var i = 0; i < figures.Length; i++)
        {
            var value = map[i];
            var player = (PlayerColor)(value & 0xFF);       // lowest 8 bits
            var isKing = ((value >> 8) & 1) != 0;    // next bit
            var figureType = (FigureId)((value >> 9) & 0xFFFF); // next 16 bits
            figures[i] = new Figure(player, isKing, figureType);
        }

        return new BoardBlueprint
        {
            Figures = figures,
            StartingPlayerColor = PlayerColor.White,
        };
    }

    public void Discard()
    {
        _boardLoader.LoadTeamBoard(_boardInfo, _maps.TeamMap);
    }

    private void MakeUnitKing(TileInfoViewModel? tile)
    {
        ArgumentNullException.ThrowIfNull(tile);

        if (tile.Figure.Owner.Equals(NeutralPlayer.Instance))
            return;

        _soundService.PlaySoundEffect(SoundEffectType.Button);
        if (tile.Figure.IsKing)
        {
            var demotedFigureId = tile.Figure.TypeInfo.FigureId;
            tile.Figure = _figureCreator.CreateFigure(new Figure(tile.Figure.Owner.PlayerColor, false, demotedFigureId));
            
            HasKing = false;
            RaisePropertyChanged(nameof(CanSave));
            return;
        }
        
        var owner = tile.Figure.Owner;
        foreach (var checkedTile in _boardInfo)
        {
            if (!checkedTile.Figure.Owner.Equals(owner) ||
                !checkedTile.Figure.IsKing) 
                continue;
            
            var demotedFigureId = checkedTile.Figure.TypeInfo.FigureId;
            checkedTile.Figure = _figureCreator.CreateFigure(new Figure(owner.PlayerColor, false, demotedFigureId));
        }
        
        var upgradedFigureId = tile.Figure.TypeInfo.FigureId;
        tile.Figure = _figureCreator.CreateFigure(new Figure(owner.PlayerColor, true, upgradedFigureId));
        
        HasKing = true;
        RaisePropertyChanged(nameof(CanSave));
    }
}