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

namespace BattleChess3.UI.Editor;

public class TeamBoardViewModel : ViewModelBase
{
    private const int BasePoints = 84;

    private bool _hasKing;
    private int _totalPoints;
    
    private readonly IFigureCreator _figureCreator;
    private readonly MapsViewModel _maps;
    private readonly IMapLoader _mapLoader;
    private readonly IMultiplayerPlayerService _playerService;
    private readonly ISoundService _soundService;

    public TeamBoardViewModel(
        IFigureCreator figureCreator,
        MapsViewModel maps,
        IMapLoader mapLoader, 
        IMultiplayerPlayerService playerService,
        ISoundService soundService)
    {
        _figureCreator = figureCreator;
        _maps = maps;
        _mapLoader = mapLoader;
        _playerService = playerService;
        _soundService = soundService;
        
        Tiles = Enumerable.Range(0, Constants.BoardLength * 2)
            .Select<int, TileViewModel>(index => new TileViewModel(Position.FromIndex(index)))
            .ToArray();
        
        Board = new Board(Tiles.Cast<ITile>().ToArray());
        mapLoader.LoadMap(Board, maps.TeamMap);
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

    public void CreateFigure(ITile tile, FigureIdentifier figureIdentifier)
    {
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = _figureCreator.CreateFigure(figureIdentifier);
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
            Figures = Tiles.Select(x => new FigureIdentifier
            {
                Player = x.Figure.Owner.Player,
                FigureId = x.Figure.Type.FigureId,
                IsKing = x.Figure.IsKing
            }).ToArray(),
        };
    }

    private void PlayerServiceOnLoggedInPlayerChanged(object? sender, EventArgs e)
    {
        var loggedInPlayer = _playerService.LoggedInPlayer;
        if (loggedInPlayer is null)
            return;
        
        var mapBlueprint = GetMapBlueprint(loggedInPlayer.Map);
        _mapLoader.LoadMap(Board, mapBlueprint);
    }
    
    private static BoardBlueprint GetMapBlueprint(byte[] map)
    {
        var figures = new FigureIdentifier[16];
        for (var i = 0; i < figures.Length; i++)
        {
            var index = i * 2;
            var playerType = PlayerSerializationHelper.ToPlayer(map[index] % 128);
            figures[i] = new FigureIdentifier(
                playerType,
                map[index + 1],
                map[index] / 128 == 1);
        }

        return new BoardBlueprint
        {
            Figures = figures,
            StartingPlayer = Player.White,
        };
    }

    public void Discard()
    {
        _mapLoader.LoadMap(Board, _maps.TeamMap);
    }

    private void MakeUnitKing(TileViewModel? tile)
    {
        ArgumentNullException.ThrowIfNull(tile);

        if (tile.Figure.Owner.Equals(NeutralPlayerInfo.Instance))
            return;

        _soundService.PlaySoundEffect(SoundEffectType.Button);
        if (tile.Figure.IsKing)
        {
            var demotedFigureId = tile.Figure.Type.FigureId;
            tile.Figure.Owner.Figures.Remove(tile.Figure);
            tile.Figure = _figureCreator.CreateFigure(new FigureIdentifier(tile.Figure.Owner.Player, demotedFigureId, false));
            
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
            checkedTile.Figure = _figureCreator.CreateFigure(new FigureIdentifier(owner.Player, demotedFigureId, false));
        }
        
        var upgradedFigureId = tile.Figure.Type.FigureId;
        tile.Figure.Owner.Figures.Remove(tile.Figure);
        tile.Figure = _figureCreator.CreateFigure(new FigureIdentifier(owner.Player, upgradedFigureId, true));
        
        HasKing = true;
        RaisePropertyChanged(nameof(CanSave));
    }
}