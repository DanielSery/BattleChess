using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.Maps;

internal class MapLoader : IMapLoader
{
    private readonly IFigureCreator _figureCreator;

    public MapLoader(IFigureCreator figureCreator)
    {
        _figureCreator = figureCreator;
    }
    
    public void LoadMap(IBoard board, MapBlueprint map)
    {
        for (var i = 0; i < board.Count; i++)
        {
            board[i].Figure = _figureCreator.CreateFigure(map.Figures[i]);
        }
    }

    public void LoadMapExtendedFor2Players(IBoard board, MapBlueprint map)
    {
        for (var i = 0; i < map.Figures.Length; i++)
        {
            var redFigure = map.Figures[i];
            var redPosition = Position.FromIndex(i + 64 - 16);
            
            var oppositeFigure = map.Figures[i].PlayerId == 0 
                ? redFigure
                : new FigureIdentifier(2, redFigure.FigureId, redFigure.IsKing);
            board[redPosition.GetPlayerPOVPosition(1)].Figure = _figureCreator.CreateFigure(oppositeFigure);
            board[redPosition.Index].Figure = _figureCreator.CreateFigure(redFigure);
        }
    }
}