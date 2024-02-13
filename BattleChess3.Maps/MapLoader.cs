using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.Maps;

public class MapLoader : IMapLoader
{
    private readonly IPlayerService _playerService;
    private readonly IFigureService _figureService;

    public MapLoader(
        IPlayerService playerService,
        IFigureService figureService)
    {
        _playerService = playerService;
        _figureService = figureService;
    }
    
    public void LoadMap(IBoard board, MapBlueprint map)
    {
        _playerService.InitializePlayers(map.StartingPlayer);

        for (var i = 0; i < IBoard.TilesCount; i++)
        {
            CreateFigure(board[i], map.Figures[i]);
        }
    }

    public void CreateFigure(ITile tile, FigureBlueprint figureBlueprint)
    {
        var figureType = _figureService.GetFigureFromName(figureBlueprint.UnitName);
        var player = _playerService.GetPlayer(figureBlueprint.PlayerId);
        var figure = new Figure(player, figureType);
        player.Figures.Add(figure);
        tile.Figure = figure;
    }
}