using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.Maps.BoardBlueprints;
using CrownsGuard.Maps.GameBoard;
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

    internal void SaveMap(IEnumerable<ITileInfo> board)
    {
        var map = new BoardBlueprint
        {
            Figures = board.Select(x => FigureHelper.GetFigure(x.Figure.Owner.PlayerColor, x.Figure.IsKing, x.Figure.TypeInfo.Figure)).ToArray(),
            StartingPlayerColor = PlayerColor.White
        };

        _boardBlueprintService.Save(map);
        TeamMap = map;
    }
}