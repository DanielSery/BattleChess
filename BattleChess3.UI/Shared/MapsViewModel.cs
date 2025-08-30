using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;
using BattleChess3.Maps;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Shared;

public sealed class MapsViewModel : ViewModelBase
{
    private readonly IMapService _mapService;

    private BoardBlueprint _teamMap;

    public MapsViewModel(IMapService mapService)
    {
        _mapService = mapService;
        _teamMap = _mapService.GetCurrentMap();
    }

    public BoardBlueprint TeamMap
    {
        get => _teamMap;
        set => SetProperty(ref _teamMap, value);
    }

    internal void SaveMap(IEnumerable<ITile> board)
    {
        var map = new BoardBlueprint
        {
            Figures = board.Select(x => new FigureIdentifier
            {
                Player = x.Figure.Owner.Player,
                FigureId = x.Figure.Type.FigureId,
                IsKing = x.Figure.IsKing
            }).ToArray(),
            StartingPlayer = Player.White
        };

        _mapService.Save(map);
        TeamMap = map;
    }
}