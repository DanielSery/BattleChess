using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.Maps;

internal class MapLoader : IMapLoader
{
    private readonly IFigureCreator _figureCreator;
    private readonly IPlayerService _playerService;

    public MapLoader(
        IPlayerService playerService,
        IFigureCreator figureCreator)
    {
        _playerService = playerService;
        _figureCreator = figureCreator;
    }
    
    public void LoadTeamMap(IBoard board, MapBlueprint map)
    {
        for (var i = 0; i < board.Count; i++)
        {
            board[i].Figure = _figureCreator.CreateFigure(map.Figures[i]);
        }
    }

    public void Load2PlayerMap(IBoard board, MapBlueprint map)
    {
        _playerService.InitializePlayers(map.StartingPlayer);
        var player1 = _playerService.GetPlayer(1);
        
        for (var i = 0; i < map.Figures.Length; i++)
        {
            var redFigure = map.Figures[i];
            var redPosition = Position.FromIndex(i + 64 - 16);
            var blueFigure = new FigureIdentifier(2, redFigure.UniqueUnitId, redFigure.IsKing);
            board[redPosition.GetPlayerPOVPosition(player1)].Figure = _figureCreator.CreateFigure(blueFigure);
            board[redPosition.Index].Figure = _figureCreator.CreateFigure(redFigure);
        }
    }
}