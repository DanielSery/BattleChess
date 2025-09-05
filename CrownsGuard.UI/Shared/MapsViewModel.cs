using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Maps;
using CrownsGuard.Maps.BoardBlueprints;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Shared;

public sealed class MapsViewModel : ViewModelBase
{
    private readonly IBoardBlueprintService _boardBlueprintService;

    private BoardBlueprint _teamMap;

    public MapsViewModel(IBoardBlueprintService boardBlueprintService)
    {
        _boardBlueprintService = boardBlueprintService;
        _teamMap = _boardBlueprintService.CurrentMap;
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
            Figures = board.Select(x => new FigureBlueprint
            {
                Player = x.Figure.Owner.Player,
                FigureId = x.Figure.Type.FigureId,
                IsKing = x.Figure.IsKing
            }).ToArray(),
            StartingPlayer = Player.White
        };

        _boardBlueprintService.Save(map);
        TeamMap = map;
    }
}