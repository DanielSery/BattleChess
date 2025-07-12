using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Maps;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Shared;

public sealed class MapsViewModel : ViewModelBase
{
    private readonly IMapService _mapService;

    private MapBlueprint _teamMap;

    public MapsViewModel(IMapService mapService)
    {
        _mapService = mapService;
        _teamMap = _mapService.GetCurrentMap();
    }

    public MapBlueprint TeamMap
    {
        get => _teamMap;
        set => SetProperty(ref _teamMap, value);
    }

    internal void SaveMap(IEnumerable<ITile> board)
    {
        var map = new MapBlueprint
        {
            Figures = board.Select(x => new FigureIdentifier
            {
                PlayerId = x.Figure.Owner.Index,
                FigureId = ((IFigureType)x.Figure).FigureId,
                IsKing = x.Figure.IsKing
            }).ToArray(),
            StartingPlayer = 1
        };

        _mapService.Save(map);
        TeamMap = map;
    }
}